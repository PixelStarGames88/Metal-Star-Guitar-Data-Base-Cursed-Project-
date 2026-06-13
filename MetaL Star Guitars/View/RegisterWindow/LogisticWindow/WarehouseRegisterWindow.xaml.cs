using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars
{
    /// <summary>
    /// Логика взаимодействия для WarehouseRegisterWindow.xaml
    /// </summary>
    public partial class WarehouseRegisterWindow : Window
    {
        private DataBaseConnector dbConnector;
        public WarehouseRegisterWindow(DataBaseConnector dateBaseConnector)
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
            _warehouseManagementWarehouseWarehouseNameTextBox.Text = string.Empty;
            _warehouseManagementWarehouseCapacityTextBox.Text = string.Empty;
            _warehouseManagementWarehouseWarehouseIdLabel.Content = ((dbConnector.Warehouses.Max(p => (int?)p.WarehouseId) ?? 0) + 1).ToString();
            _warehouseManagementWarehouseWarehouseRegisterButtonLabel.Content = "Register";
        }

       
        private void warehouseManagementWarehouseWarehouseRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int capacity;

            if (string.IsNullOrEmpty(_warehouseManagementWarehouseWarehouseNameTextBox.Text))
            {
                new MessageWindow("Error", "Enter warehouse name!").Show();
                return;
            }
            if (dbConnector.Warehouses.Any(w => w.WarehouseName == _warehouseManagementWarehouseWarehouseNameTextBox.Text) &&
                !dbConnector.Warehouses.Any(w => w.WarehouseId == int.Parse(_warehouseManagementWarehouseWarehouseIdLabel.Content.ToString()!)))
            {
                new MessageWindow("Error", "Warehouse name must be unique!").Show();
                return;
            }
            if (!int.TryParse(_warehouseManagementWarehouseCapacityTextBox.Text, out capacity))
            {
                new MessageWindow("Error", "Enter integer number\nin field for capacity!").Show();
                return;
            }

            if (!dbConnector.Stocks.Any(w => w.WarehouseId == int.Parse(_warehouseManagementWarehouseWarehouseIdLabel.Content.ToString()!)))
                addNewWarehouse(_warehouseManagementWarehouseWarehouseNameTextBox.Text, capacity);
            else
                updateWarehouse(int.Parse(_warehouseManagementWarehouseWarehouseIdLabel.Content.ToString()!), _warehouseManagementWarehouseWarehouseNameTextBox.Text, capacity);
            defaultState();
            new MessageWindow("Message", "Changes are successfull!").Show();
        }

        private void updateWarehouse(int warehouseId, string warehouseName, int capacity)
        {
            DataBase.Entities.warehouse_entity warehouse = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseId == warehouseId)!;
            warehouse.WarehouseName = warehouseName;
            warehouse.Capacity = capacity;
            dbConnector.SaveChanges();
        }
        private void addNewWarehouse(string warehouseName, int capacity)
        {
            dbConnector.Warehouses.Add
            (
                new DataBase.Entities.warehouse_entity
                {
                    WarehouseName = warehouseName,
                    Capacity = capacity
                }
            );
            dbConnector.SaveChanges();
        }
        private void warehouseManagementWarehouseWarehouseCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _warehouseManagementWarehouseWarehouseNameTextBox.Text = string.Empty;
            _warehouseManagementWarehouseCapacityTextBox.Text = string.Empty;
            _warehouseManagementWarehouseWarehouseIdLabel.Content = ((dbConnector.Warehouses.Max(p => (int?)p.WarehouseId) ?? 0) + 1).ToString();
        }
    }
}
