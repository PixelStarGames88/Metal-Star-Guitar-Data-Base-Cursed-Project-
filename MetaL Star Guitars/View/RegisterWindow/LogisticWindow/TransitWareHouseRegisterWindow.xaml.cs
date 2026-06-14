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
    /// Логика взаимодействия для TransitWareHouseRegisterWindow.xaml
    /// </summary>
    public partial class TransitWareHouseRegisterWindow : Window
    {
        private DataBaseConnector dbConnector;

        public TransitWareHouseRegisterWindow(DataBaseConnector dateBaseConnector)
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
            _warehouseManagementTransitWarehouseTypeTextBox.Text = string.Empty;
            _warehouseManagementTransitWarehouseCapacityTextBox.Text = string.Empty;
            _warehouseManagementTransitWarehouseIdLabel.Content = ((dbConnector.TransitWarehouses.Max(p => (int?)p.TransitWarehouseId) ?? 0) + 1).ToString();
            _warehouseManagementTransitWarehouseRegisterButtonLabel.Content = "Register";
            fill_routesListBox();
        }
        private void fill_routesListBox()
        {
            var rawData = dbConnector.Routes
                .Select(x => new
                {
                    IdRoute = x.RouteId,
                    TransitWarehouseDescribe = ("ID: " + x.RouteId.ToString() + "\nTravel Time: " + x.TravelTime.ToString())
                })
                .ToList();

            var routes = rawData.Select(x => {
                dynamic item = new ExpandoObject();
                item.IdRoute = x.IdRoute;
                item.TransitWarehouseDescribe = x.TransitWarehouseDescribe;
                item.IsSelected = false;
                return (object)item;
            }).ToList();

            _routesListBox.ItemsSource = routes;
        }

        private void _warehouseManagementTransitWarehouseCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            defaultState();
        }

        private void _warehouseManagementTransitWarehouseRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_warehouseManagementTransitWarehouseTypeTextBox.Text))
            {
                new MessageWindow("Error", "Please enter a transit type.").Show();
                return;
            }

            int capacity;
            if (!int.TryParse(_warehouseManagementTransitWarehouseCapacityTextBox.Text.Trim(), out capacity))
            {
                new MessageWindow("Error", "Please enter a valid capacity.").Show();
                return;
            }

            int transitWarehouseId = int.Parse(_warehouseManagementTransitWarehouseIdLabel.Content.ToString()!);

            if (_warehouseManagementTransitWarehouseRegisterButtonLabel.Content.ToString() == "Save")
                update_TransitWarehouse(transitWarehouseId);
            else
                add_newTransitWarehouse(transitWarehouseId);

            add_newTransitWarehouseRoutes(transitWarehouseId);

            this.Close();
        }

        private void add_newTransitWarehouse(int transitWarehouseId)
        {
            string typeTransit = _warehouseManagementTransitWarehouseTypeTextBox.Text.Trim();
            int capacity = int.Parse(_warehouseManagementTransitWarehouseCapacityTextBox.Text.Trim());

            dbConnector.TransitWarehouses.Add(new transit_warehouse_entity
            {
                TransitWarehouseId = transitWarehouseId,
                TypeTransit = typeTransit,
                Capacity = capacity,
                RouteId = 0
            });

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.fill_TransitWarehouseListBox();
            new MessageWindow("Message", "Changes are successfull!").Show();
            dbConnector.SaveChanges();
            this.Close();
        }

        private void update_TransitWarehouse(int transitWarehouseId)
        {
            string typeTransit = _warehouseManagementTransitWarehouseTypeTextBox.Text.Trim();
            int capacity = int.Parse(_warehouseManagementTransitWarehouseCapacityTextBox.Text.Trim());

            var existingEntity = dbConnector.TransitWarehouses.FirstOrDefault(x => x.TransitWarehouseId == transitWarehouseId);
            if (existingEntity != null)
            {
                existingEntity.TypeTransit = typeTransit;
                existingEntity.Capacity = capacity;
            }

            var oldRouteLinks = dbConnector.TransitWarehouseRoutes.Where(x => x.TransitWarehouseId == transitWarehouseId).ToList();
            dbConnector.TransitWarehouseRoutes.RemoveRange(oldRouteLinks);

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.fill_TransitWarehouseListBox();
            new MessageWindow("Message", "Changes are successfull!").Show();
            dbConnector.SaveChanges();
            this.Close();
        }

        private void add_newTransitWarehouseRoutes(int transitWarehouseId)
        {
            var selectedRoutes = _routesListBox.Items.Cast<dynamic>()
                .Where(x => x.IsSelected)
                .Select(x => new transit_warehouse_route_entity
                {
                    TransitWarehouseId = transitWarehouseId,
                    RouteId = (int)x.IdRoute
                })
                .ToList();

            dbConnector.TransitWarehouseRoutes.AddRange(selectedRoutes);
            dbConnector.SaveChanges();
        }
    }
}