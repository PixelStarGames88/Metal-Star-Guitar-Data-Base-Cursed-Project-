using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow
{
    /// <summary>
    /// Логика взаимодействия для TransferOrderRegisterWindow.xaml
    /// </summary>
    public partial class TransferOrderRegisterWindow : Window
    {
        private DataBaseConnector dbConnector;
        public TransferOrderRegisterWindow(DataBaseConnector dateBaseConnector)
        {
            InitializeComponent();
            dbConnector = dateBaseConnector;
            defaultState();
        }
        private void someButton_MouseEnterYellow(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.Yellow;
        }
        private void someButton_MouseLeaveWhite(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.White;
        }
        private void defaultState()
        {
            _warehouseManagementTransfersRouteComboBox.Text = string.Empty;
            _warehouseManagementTransfersSenderWarehouseComboBox.Text = string.Empty;
            _warehouseManagementTransfersRecipientWarehouseComboBox.Text = string.Empty;
            int maxId = dbConnector.TransferOrders.Max(p => (int?)p.TransferOrderId) ?? 0;
            _warehouseManagementTransfersOrderIdLabel.Content = (maxId + 1).ToString();
            _warehouseManagementTransfersShipmentDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
            _warehouseManagementTransfersReceiptDateLabel.Content = "Choice route.";
            fillRecepientWarehouseComboBox();
            fillSenderWarehouseComboBox();
        }
        private void fillRecepientWarehouseComboBox()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _warehouseManagementTransfersRecipientWarehouseComboBox.ItemsSource = warehouses;
            _warehouseManagementTransfersRecipientWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }
        private void fillSenderWarehouseComboBox()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _warehouseManagementTransfersSenderWarehouseComboBox.ItemsSource = warehouses;
            _warehouseManagementTransfersSenderWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }
        private void warehouseManagementTransfersCancelButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            defaultState();
        }
        private void warehouseManagementTransfersSenderWarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fill_productListBox();
            fillRoutesComboBox();
        }
        private void warehouseManagementTransfersRecipientWarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillRoutesComboBox();
        }
        private void warehouseManagementTransfersToOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem == null)
            {
                new MessageWindow("Error", "Choice warehouse sender!").Show();
                return;
            }
            if (_warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem == null)
            {
                new MessageWindow("Error", "Choice warehouse recepient!").Show();
                return;
            }
            if (_warehouseManagementTransfersRouteComboBox.SelectedItem == null)
            {
                new MessageWindow("Error", "Choice route!").Show();
                return;
            }

            int transferOrderId = int.Parse(_warehouseManagementTransfersOrderIdLabel.Content.ToString()!);

            if (_warehouseManagementTransfersToOrderButton.Content.ToString() == "Save")
                update_TransferOrder(transferOrderId);
            else
                add_newTransferOrder(transferOrderId);

            add_newContents(transferOrderId);

            warehouseManagementTransfersCancelButton_MouseDown(sender, e);
            new MessageWindow("Message", "Your order was made successfully!").Show();
        }
        private void add_newTransferOrder(int transferOrderId)
        {
            int senderWarehouseId = (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            int recipientWarehouseId = (_warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            dynamic selectedItem = _warehouseManagementTransfersRouteComboBox.SelectedItem ?? throw new NullReferenceException();
            string finalRouteId = selectedItem.FinalRoute.FinalRouteId;

            DateTime shipmentDate = DateTime.UtcNow;
            DateTime receiptDate = DateTime.UtcNow + selectedItem.FinalRoute.ScheduledTime;

            dbConnector.TransferOrders.Add(new transfer_order_entity
            {
                TransferOrderId = transferOrderId,
                SenderWarehouseId = senderWarehouseId,
                RecipientWarehouseId = recipientWarehouseId,
                ShipmentDate = shipmentDate,
                EstimatedDeliveryDate = receiptDate,
                Status = "Open",
                FinalRouteId = finalRouteId
            });

            dbConnector.SaveChanges();
        }
        private void update_TransferOrder(int transferOrderId)
        {
            int senderWarehouseId = (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            int recipientWarehouseId = (_warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            dynamic selectedItem = _warehouseManagementTransfersRouteComboBox.SelectedItem ?? throw new NullReferenceException();
            string finalRouteId = selectedItem.FinalRoute.FinalRouteId;

            var existingOrder = dbConnector.TransferOrders.FirstOrDefault(x => x.TransferOrderId == transferOrderId);
            if (existingOrder != null)
            {
                existingOrder.SenderWarehouseId = senderWarehouseId;
                existingOrder.RecipientWarehouseId = recipientWarehouseId;
                existingOrder.FinalRouteId = finalRouteId;
            }

            var oldContent = dbConnector.TransferOrderContents.Where(x => x.TransferOrderId == transferOrderId).ToList();
            dbConnector.TransferOrderContents.RemoveRange(oldContent);

            dbConnector.SaveChanges();
        }
        private void add_newContents(int transferOrderId)
        {
            foreach (var item in _contentListBox.Items)
            {
                if (item == null) continue;

                dynamic productItem = item;
                if (!productItem.IsSelected) continue;

                int productId = productItem.ProductId;
                int quantity = int.Parse(productItem.Quantity);

                if (quantity <= 0)
                {
                    new MessageWindow("Error", "Enter integer number in text boxes for quantity!").Show();
                    return;
                }

                dbConnector.TransferOrderContents.Add(new transfer_order_content_entity
                {
                    TransferOrderId = transferOrderId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            dbConnector.SaveChanges();
        }
        private void fill_productListBox()
        {
            if (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem is not warehouse_entity senderWarehouse)
                return;

            string warehousename = senderWarehouse.WarehouseName;

            var rawData = dbConnector.Products
                .Join
                (
                    dbConnector.Stocks, p => p.ProductId, s => s.ProductId, 
                    (p, s) => new 
                    { 
                        Product = p, 
                        Stock = s 
                    })
                .Join
                (
                    dbConnector.Warehouses, combined => combined.Stock.WarehouseId, w => w.WarehouseId,
                    (combined, w) => new 
                    { 
                        combined.Product, 
                        Warehouse = w, 
                        ProductDescribe = combined.Product.ProductName + " (" + combined.Stock.Quantity.ToString() + " pcs.)" 
                    })
                .Where(x => x.Warehouse.WarehouseName == warehousename)
                .Select(x => new { x.ProductDescribe, x.Product.ProductId })
                .Distinct()
                .ToList();

            var products = rawData.Select(x => {
                dynamic item = new ExpandoObject();
                item.ProductDescribe = x.ProductDescribe;
                item.ProductId = x.ProductId;
                item.IsSelected = false;
                item.Quantity = 0;
                return (object)item;
            }).ToList();

            _contentListBox.ItemsSource = products;
        }
        private void warehouseManagementTransfersRouteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_warehouseManagementTransfersRouteComboBox.SelectedItem == null)
            {
                _warehouseManagementTransfersReceiptDateLabel.Content = "Choice route.";
                return;
            }
            dynamic selectedItem = _warehouseManagementTransfersRouteComboBox.SelectedItem ?? throw new NullReferenceException();
            _warehouseManagementTransfersReceiptDateLabel.Content = (DateTime.Now + selectedItem.FinalRoute.ScheduledTime).ToString("HH\\:mm dd.MM.yyyy");
        }
        private void fillRoutesComboBox()
        {
            if (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem == null ||
                _warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem == null)
                return;

            _warehouseManagementTransfersRouteComboBox.ItemsSource = null;

            string warehousename_1 = (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();
            string warehousename_2 = (_warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();

            CreateRoutes_1(warehousename_1, warehousename_2);
            CreateRoutes_2(warehousename_1, warehousename_2);
            CreateRoutes_3(warehousename_1, warehousename_2);

            var FinalRoutes = dbConnector.FinalRoutes
                .Join
                (
                    dbConnector.RouteFinalRoutes, fr => fr.FinalRouteId, rfr => rfr.FinalRouteId,
                    (fr, rfr) => new
                    {
                        FinalRoute = fr,
                        RouteFinalRoute = rfr
                    }
                )
                .Join
                (
                    dbConnector.RouteFinalRoutes, fr => fr.FinalRoute.FinalRouteId, rfr => rfr.FinalRouteId,
                    (fr, rfr) => new
                    {
                        FinalRoute = fr.FinalRoute,
                        StartRouteFinalRoute = fr.RouteFinalRoute,
                        FinishRouteFinalRoute = rfr
                    }
                )
                .Join
                (
                    dbConnector.Routes,
                    fr => fr.StartRouteFinalRoute.RouteId,
                    r => r.RouteId,
                    (fr, r) => new
                    {
                        FinalRoute = fr.FinalRoute,
                        StartRouteFinalRoute = fr.StartRouteFinalRoute,
                        FinishRouteFinalRoute = fr.FinishRouteFinalRoute,
                        Start_Route = r
                    }
                )
                .Join
                (
                    dbConnector.Routes,
                    fr => fr.FinishRouteFinalRoute.RouteId,
                    r => r.RouteId,
                    (fr, r) => new
                    {
                        FinalRoute = fr.FinalRoute,
                        StartRouteFinalRoute = fr.StartRouteFinalRoute,
                        FinishRouteFinalRoute = fr.FinishRouteFinalRoute,
                        Start_Route = fr.Start_Route,
                        Finish_Route = r
                    }
                )
                .Join
                (
                    dbConnector.Warehouses, fr => fr.Start_Route.StartWarehouseId, w => w.WarehouseId,
                    (fr, w) => new
                    {
                        FinalRoute = fr.FinalRoute,
                        StartRouteFinalRoute = fr.StartRouteFinalRoute,
                        FinishRouteFinalRoute = fr.FinishRouteFinalRoute,
                        Start_Route = fr.Start_Route,
                        Finish_Route = fr.Finish_Route,
                        StartWarehouse = w
                    }
                )
                 .Join
                (
                    dbConnector.Warehouses, fr => fr.Finish_Route.FinishWarehouseId, w => w.WarehouseId,
                    (fr, w) => new
                    {
                        FinalRoute = fr.FinalRoute,
                        StartRouteFinalRoute = fr.StartRouteFinalRoute,
                        FinishRouteFinalRoute = fr.FinishRouteFinalRoute,
                        Start_Route = fr.Start_Route,
                        Finish_Route = fr.Finish_Route,
                        StartWarehouse = fr.StartWarehouse,
                        FinishWarehouse = w,
                        Discribe = ("From " + fr.StartWarehouse.WarehouseName + " to " + w.WarehouseName + " in " + fr.FinalRoute.ScheduledTime.ToString(@"dd\.hh\:mm"))
                    }
                )
                .ToList().Where(
                n =>
                n.Start_Route.RouteId == int.Parse(n.FinalRoute.FinalRouteId!.Split('_')[0]) &&
                n.Finish_Route.RouteId == int.Parse(n.FinalRoute.FinalRouteId.Split('_')[^1]) &&
                n.StartWarehouse.WarehouseName == warehousename_1 && n.FinishWarehouse.WarehouseName == warehousename_2);

            _warehouseManagementTransfersRouteComboBox.ItemsSource = FinalRoutes;
            _warehouseManagementTransfersRouteComboBox.DisplayMemberPath = "Discribe";

        }
        private void CreateRoutes_1(string warehousename_1, string warehousename_2)
        {
            var FinalRoutes =
                dbConnector.Warehouses
                .Join
                (
                    dbConnector.Routes, w => w.WarehouseId, r => r.StartWarehouseId,
                    (w, r) => new { Start = w, Route = r }
                )
                .Join
                (
                    dbConnector.Warehouses, wr => wr.Route.FinishWarehouseId, w => w.WarehouseId,
                    (wr, w) => new
                    {
                        warehouse_1 = wr.Start.WarehouseName,
                        route_1 = wr.Route,
                        warehouse_2 = w.WarehouseName
                    }
                ).Where
                (
                    n => n.warehouse_1 == warehousename_1 && n.warehouse_2 == warehousename_2
                )
                .ToList();

            foreach (var FinalRoute in FinalRoutes)
            {
                if (dbConnector.FinalRoutes.Any(n => n.FinalRouteId == (FinalRoute.route_1.RouteId.ToString())))
                    continue;

                dbConnector.FinalRoutes.Add(new final_route_entity
                {
                    ScheduledTime = FinalRoute.route_1.TravelTime,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString())
                }
                );
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 1,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString()),
                    RouteId = FinalRoute.route_1.RouteId
                });

            }
            dbConnector.SaveChanges();
        }
        private void CreateRoutes_2(string warehousename_1, string warehousename_2)
        {
            var FinalRoutes =
                dbConnector.Warehouses
                .Join

                (
                    dbConnector.Routes, w => w.WarehouseId, r => r.StartWarehouseId,
                    (w, r) => new { Start = w, Route = r }
                )
                .Join
                (
                    dbConnector.Warehouses, wr => wr.Route.FinishWarehouseId, w => w.WarehouseId,
                    (wr, w) => new
                    {
                        warehouse_1 = wr.Start,
                        route_1 = wr.Route,
                        warehouse_2 = w,

                    }
                )
                .Join
                (
                    dbConnector.Routes, wrw => wrw.warehouse_2.WarehouseId, r => r.StartWarehouseId,
                    (wrw, r) => new
                    {
                        warehouse_1 = wrw.warehouse_1,
                        route_1 = wrw.route_1,
                        warehouse_2 = wrw.warehouse_2,
                        route_2 = r
                    }
                )
                .Join
                (
                    dbConnector.Warehouses, wrwr => wrwr.route_2.FinishWarehouseId, w => w.WarehouseId,
                    (wrwr, w) => new
                    {
                        warehouse_1 = wrwr.warehouse_1.WarehouseName,
                        route_1 = wrwr.route_1,
                        route_2 = wrwr.route_2,
                        warehouse_2 = w.WarehouseName
                    }
                ).Where
                (
                    n => n.warehouse_1 == warehousename_1 && n.warehouse_2 == warehousename_2
                )
                .ToList();


            foreach (var FinalRoute in FinalRoutes)
            {
                if (dbConnector.FinalRoutes.Any(n => n.FinalRouteId == (FinalRoute.route_1.RouteId.ToString() + "_" +
                FinalRoute.route_2.RouteId.ToString())))
                    continue;

                dbConnector.FinalRoutes.Add(new final_route_entity
                {
                    ScheduledTime = (FinalRoute.route_1.TravelTime + FinalRoute.route_2.TravelTime),
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString()),
                }
            );
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 1,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString()),
                    RouteId = FinalRoute.route_1.RouteId
                });
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 2,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString()),
                    RouteId = FinalRoute.route_2.RouteId
                });
            }
            dbConnector.SaveChanges();
        }
        private void CreateRoutes_3(string warehousename_1, string warehousename_2)
        {
            var FinalRoutes =
                dbConnector.Warehouses
                .Join
                (
                    dbConnector.Routes, w => w.WarehouseId, r => r.StartWarehouseId,
                    (w, r) => new { Start = w, Route = r }
                )
                .Join
                (
                    dbConnector.Warehouses, wr => wr.Route.FinishWarehouseId, w => w.WarehouseId,
                    (wr, w) => new
                    {
                        warehouse_1 = wr.Start,
                        route_1 = wr.Route,
                        warehouse_2 = w,
                    }
                )
                .Join
                (
                    dbConnector.Routes, wrw => wrw.warehouse_2.WarehouseId, r => r.StartWarehouseId,
                    (wrw, r) => new
                    {
                        warehouse_1 = wrw.warehouse_1,
                        route_1 = wrw.route_1,
                        warehouse_2 = wrw.warehouse_2,
                        route_2 = r
                    }
                )
                .Join
                (
                    dbConnector.Warehouses, wrwr => wrwr.route_2.FinishWarehouseId, w => w.WarehouseId,
                    (wrwr, w) => new
                    {
                        warehouse_1 = wrwr.warehouse_1,
                        route_1 = wrwr.route_1,
                        warehouse_2 = wrwr.warehouse_2,
                        route_2 = wrwr.route_2,
                        warehouse_3 = w
                    }
                )
                .Join
                (
                    dbConnector.Routes, wrwrw => wrwrw.warehouse_3.WarehouseId, r => r.StartWarehouseId,
                    (wrwrw, r) => new
                    {
                        warehouse_1 = wrwrw.warehouse_1,
                        route_1 = wrwrw.route_1,
                        warehouse_2 = wrwrw.warehouse_2,
                        route_2 = wrwrw.route_2,
                        warehouse_3 = wrwrw.warehouse_3,
                        route_3 = r
                    }
                )
                .Join
                (
                    dbConnector.Warehouses, wrwrwr => wrwrwr.route_3.FinishWarehouseId, w => w.WarehouseId,
                    (wrwrwr, w) => new
                    {
                        warehouse_1 = wrwrwr.warehouse_1.WarehouseName,
                        route_1 = wrwrwr.route_1,
                        route_2 = wrwrwr.route_2,
                        route_3 = wrwrwr.route_3,
                        warehouse_2 = w.WarehouseName
                    }
                ).Where
                (
                    n => n.warehouse_1 == warehousename_1 && n.warehouse_2 == warehousename_2
                )
                .ToList();

            foreach (var FinalRoute in FinalRoutes)
            {
                if (dbConnector.FinalRoutes.Any(n => n.FinalRouteId ==
                (FinalRoute.route_1.RouteId.ToString() + "_" +
                FinalRoute.route_2.RouteId.ToString() + "_" +
                FinalRoute.route_3.RouteId.ToString())))
                    continue;

                dbConnector.FinalRoutes.Add(new final_route_entity
                {
                    ScheduledTime = (FinalRoute.route_1.TravelTime + FinalRoute.route_2.TravelTime + FinalRoute.route_3.TravelTime),
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString() + "_" + FinalRoute.route_3.RouteId.ToString()),
                }
                );
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 1,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString() + "_" + FinalRoute.route_3.RouteId.ToString()),
                    RouteId = FinalRoute.route_1.RouteId
                });
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 2,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString() + "_" + FinalRoute.route_3.RouteId.ToString()),
                    RouteId = FinalRoute.route_2.RouteId
                });
                dbConnector.RouteFinalRoutes.Add(new route_final_route_entity
                {
                    SequenceRoute = 3,
                    FinalRouteId = (FinalRoute.route_1.RouteId.ToString() + "_" + FinalRoute.route_2.RouteId.ToString() + "_" + FinalRoute.route_3.RouteId.ToString()),
                    RouteId = FinalRoute.route_3.RouteId
                });
            }
            dbConnector.SaveChanges();
        }
    }
}