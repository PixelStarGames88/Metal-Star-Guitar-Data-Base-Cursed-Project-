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
    /// Логика взаимодействия для RouteRegisterWindow.xaml
    /// </summary>
    public partial class RouteRegisterWindow : Window
    {
        private DataBaseConnector dbConnector;

        public RouteRegisterWindow(DataBaseConnector dateBaseConnector)
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
            _warehouseManagementRouteStartWarehouseComboBox.Text = string.Empty;
            _warehouseManagementRouteFinishWarehouseComboBox.Text = string.Empty;
            _warehouseManagementRouteTimeTextBox.Text = string.Empty;
            _warehouseManagementRouteRouteIdLabel.Content = ((dbConnector.Routes.Max(p => (int?)p.RouteId) ?? 0) + 1).ToString();
            _warehouseManagementRouteRegisterButtonLabel.Content = "Register";
            fill_transitWarehousesListBox();
            fill_warehouseManagementRouteStartWarehouseComboBox();
            fill_warehouseManagementRouteFinishWarehouseComboBox();
        }
        private void fill_transitWarehousesListBox()
        {
            var rawData = dbConnector.TransitWarehouses
                .Select(x => new
                {
                    IdWarehouse = x.TransitWarehouseId,
                    TransitWarehouseDescribe = ("ID: " + x.TransitWarehouseId.ToString() + "\nType: " + x.TypeTransit.ToString() + "\nCapacity: " + x.Capacity.ToString())
                })
                .ToList();

            var transitWarehouses = rawData.Select(x => {
                dynamic item = new ExpandoObject();
                item.IdWarehouse = x.IdWarehouse;
                item.TransitWarehouseDescribe = x.TransitWarehouseDescribe;
                item.IsSelected = false;
                return (object)item;
            }).ToList();

            _transitWarehousesListBox.ItemsSource = transitWarehouses;
        }
        private void fill_warehouseManagementRouteStartWarehouseComboBox()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _warehouseManagementRouteStartWarehouseComboBox.ItemsSource = warehouses;
            _warehouseManagementRouteStartWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }
        private void fill_warehouseManagementRouteFinishWarehouseComboBox()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _warehouseManagementRouteFinishWarehouseComboBox.ItemsSource = warehouses;
            _warehouseManagementRouteFinishWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }

        private void warehouseManagementWarehouseWarehouseCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            defaultState();
        }

        private void _warehouseManagementRouteRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_warehouseManagementRouteStartWarehouseComboBox.SelectedItem == null)
            {
                new MessageWindow("Error", "Please select a starting warehouse.").Show();
                return;
            }
            if (_warehouseManagementRouteFinishWarehouseComboBox.SelectedItem == null)
            {
                new MessageWindow("Error", "Please select a destination warehouse.").Show();
                return;
            }
            if (string.IsNullOrWhiteSpace(_warehouseManagementRouteTimeTextBox.Text))
            {
                new MessageWindow("Error", "Please enter a valid travel time.").Show();
                return;
            }

            int routeId = int.Parse(_warehouseManagementRouteRouteIdLabel.Content.ToString()!);

            if (_warehouseManagementRouteRegisterButtonLabel.Content.ToString() == "Save")
                update_Route(routeId);
            else
                add_newRoute(routeId);

            add_newRoutesTransitWarehouses(routeId);

            this.Close();
        }

        private void add_newRoute(int routeId)
        {
            TimeSpan travelTime;
            if (!TimeSpan.TryParse(_warehouseManagementRouteTimeTextBox.Text.Trim(), out travelTime))
            {
                new MessageWindow("Error", "Please enter a valid travel time in hours.").Show();
                return;
            }

            int startWarehouseId = (_warehouseManagementRouteStartWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            int finishWarehouseId = (_warehouseManagementRouteFinishWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();

            dbConnector.Routes.Add(new route_entity
            {
                RouteId = routeId,
                StartWarehouseId = startWarehouseId,
                FinishWarehouseId = finishWarehouseId,
                TravelTime = travelTime
            });

            add_newRoutesTransitWarehouses(routeId);

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.fill_RouteListBox();
            new MessageWindow("Message", "Changes are successfull!").Show();
            dbConnector.SaveChanges();
            this.Close();
        }

        private void update_Route(int routeId)
        {
            TimeSpan travelTime;
            if (!TimeSpan.TryParse(_warehouseManagementRouteTimeTextBox.Text.Trim(), out travelTime))
            {
                new MessageWindow("Error", "Please enter a valid travel time in hours.").Show();
                return;
            }

            int startWarehouseId = (_warehouseManagementRouteStartWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            int finishWarehouseId = (_warehouseManagementRouteFinishWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();

            var existingRoute = dbConnector.Routes.FirstOrDefault(x => x.RouteId == routeId);
            if (existingRoute != null)
            {
                existingRoute.StartWarehouseId = startWarehouseId;
                existingRoute.FinishWarehouseId = finishWarehouseId;
                existingRoute.TravelTime = travelTime;
            }

            var oldTransitLinks = dbConnector.TransitWarehouseRoutes.Where(x => x.RouteId == routeId).ToList();
            dbConnector.TransitWarehouseRoutes.RemoveRange(oldTransitLinks);

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.fill_RouteListBox();
            new MessageWindow("Message", "Changes are successfull!").Show();
            dbConnector.SaveChanges();
            this.Close();
        }

        private void add_newRoutesTransitWarehouses(int routeId)
        {
            var selectedTransits = _transitWarehousesListBox.Items.Cast<dynamic>()
                .Where(x => x.IsSelected)
                .Select(x => new transit_warehouse_route_entity
                {
                    RouteId = routeId,
                    TransitWarehouseId = (int)x.IdWarehouse
                })
                .ToList();

            dbConnector.TransitWarehouseRoutes.AddRange(selectedTransits);
            
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.fill_RouteListBox();
            new MessageWindow("Message", "Changes are successfull!").Show();
            dbConnector.SaveChanges();
            this.Close();
        }
    }
}