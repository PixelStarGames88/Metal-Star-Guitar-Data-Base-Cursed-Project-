using MetaL_Star_Guitars.DataBase.Entities;
using MetaL_Star_Guitars.View.RegisterWindow;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementTransfersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementTransfersGrid.Visibility = Visibility.Visible;

        fill_TransferOrderListBox();
    }
    private void fill_TransferOrderListBox()
    {
        var documents = dbConnector.TransferOrders
            .Join
            (
                dbConnector.Warehouses, to => to.SenderWarehouseId, w => w.WarehouseId,
                (to, w) => new
                {
                    order = to,
                    senderWarehouse = w
                }
            )
            .Join
            (
                dbConnector.Warehouses, to => to.order.RecipientWarehouseId, w => w.WarehouseId,
                (to, w) => new
                {
                    orderId = to.order.TransferOrderId,
                    orderStartData = to.order.ShipmentDate,
                    orderStatus = to.order.Status,
                    orderFinishData = to.order.EstimatedDeliveryDate,
                    senderWarehouse = to.senderWarehouse.WarehouseName,
                    recipientWarehouse = w.WarehouseName
                }
            ).ToList();

        _transferOrdersListBox.ItemsSource = documents;
    }
    private void editTransferOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;
        var entity = dbConnector.TransferOrders.FirstOrDefault(x => x.TransferOrderId == orderId);

        if (entity != null)
        {
            var w = new TransferOrderRegisterWindow(dbConnector);

            w._warehouseManagementTransfersOrderIdLabel.Content = entity.TransferOrderId;
            w._warehouseManagementTransfersStatusLabel.Content = entity.Status;
            w._warehouseManagementTransfersShipmentDateLabel.Content = entity.ShipmentDate.ToString("HH\\:mm dd.MM.yyyy");
            w._warehouseManagementTransfersReceiptDateLabel.Content = entity.EstimatedDeliveryDate.ToString("HH\\:mm dd.MM.yyyy");

            var senderWarehouse = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == entity.SenderWarehouseId);
            var recipientWarehouse = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == entity.RecipientWarehouseId);

            w._warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem = senderWarehouse;
            w._warehouseManagementTransfersRecipientWarehouseComboBox.SelectedItem = recipientWarehouse;

            if (entity.FinalRouteId != null)
            {
                foreach (var routeItem in w._warehouseManagementTransfersRouteComboBox.Items)
                {
                    dynamic route = routeItem;
                    if (route.FinalRoute.FinalRouteId == entity.FinalRouteId)
                    {
                        w._warehouseManagementTransfersRouteComboBox.SelectedItem = routeItem;
                        break;
                    }
                }
            }

            var orderContents = dbConnector.TransferOrderContents
                .Where(c => c.TransferOrderId == orderId)
                .ToList();

            if (w._warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem is warehouse_entity senderWh)
            {
                var rawData = dbConnector.Products
                    .Join(dbConnector.Stocks, p => p.ProductId, s => s.ProductId, (p, s) => new { Product = p, Stock = s })
                    .Join(dbConnector.Warehouses, combined => combined.Stock.WarehouseId, w => w.WarehouseId,
                        (combined, w) => new { combined.Product, Warehouse = w, ProductDescribe = combined.Product.ProductName + " (" + combined.Stock.Quantity.ToString() + " pcs.)" })
                    .Where(x => x.Warehouse.WarehouseName == senderWh.WarehouseName)
                    .Select(x => new { x.ProductDescribe, x.Product.ProductId })
                    .Distinct()
                    .ToList();

                var products = rawData.Select(x => {
                    dynamic expando = new ExpandoObject();
                    expando.ProductDescribe = x.ProductDescribe;
                    expando.ProductId = x.ProductId;

                    var content = orderContents.FirstOrDefault(c => c.ProductId == x.ProductId);

                    expando.IsSelected = content != null;
                    expando.Quantity = content?.Quantity ?? 0;

                    return (object)expando;
                }).ToList();

                w._contentListBox.ItemsSource = products;
            }

            w._warehouseManagementTransfersToOrderButton.Content = "Save";
            w.Show();
        }
    }
    private void deleteTransferOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;
        var entity = dbConnector.TransferOrders.FirstOrDefault(x => x.TransferOrderId == orderId);
        if (entity != null) { dbConnector.TransferOrders.Remove(entity); dbConnector.SaveChanges(); fill_TransferOrderListBox(); }
    }
    private void _createTransferOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new TransferOrderRegisterWindow(dbConnector).Show();
    }
}