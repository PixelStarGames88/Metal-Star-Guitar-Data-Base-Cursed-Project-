using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow.LogisticWindow;

/// <summary>
/// Логика взаимодействия для StockRegisterWindow.xaml
/// </summary>
public partial class StockRegisterWindow : Window
{
    private DataBaseConnector dbConnector;
    public StockRegisterWindow(DataBaseConnector dateBaseConnector)
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
        _warehouseManagementWarehousesQuantityTextBox.Text = string.Empty;
        _warehouseManagementWarehouseWarehouseComboBox.Text = string.Empty;
        _warehouseManagementWarehouseProductComboBox.Text = string.Empty;
        fill_warehouseManagementWarehouseWarehouseComboBox();
        fill_warehouseManagementWarehouseProductComboBox();
        _warehouseManagementWarehouseStockRegisterButtonLabel.Content = "Register";
    }

    private void fill_warehouseManagementWarehouseWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementWarehouseWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementWarehouseWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void fill_warehouseManagementWarehouseProductComboBox()
    {
        var products = dbConnector.Products.ToList();
        _warehouseManagementWarehouseProductComboBox.ItemsSource = products;
        _warehouseManagementWarehouseProductComboBox.DisplayMemberPath = "ProductName";
    }

    private void updateWarehouse(int warehouseId, string warehouseName, int capacity)
    {
        DataBase.Entities.warehouse_entity warehouse = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseId == warehouseId)!;
        warehouse.WarehouseName = warehouseName;
        warehouse.Capacity = capacity;

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_StockListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();

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
        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_StockListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }

    private void warehouseManagementWarehouseStockRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        int quantity;
        if (_warehouseManagementWarehouseWarehouseComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choise warehouse!").Show();
            return;
        }
        if (_warehouseManagementWarehouseProductComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choise product!").Show();
            return;
        }
        if (!int.TryParse(_warehouseManagementWarehousesQuantityTextBox.Text, out quantity))
        {
            new MessageWindow("Error", "Enter integer number\nin field for quantity!").Show();
            return;
        }

        int productId = (_warehouseManagementWarehouseProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int warehouseId = (_warehouseManagementWarehouseWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();

        if (!dbConnector.Stocks.Any(w => w.WarehouseId == warehouseId && w.ProductId == productId))
            addNewStock(warehouseId, productId, quantity);
        else
            updateStock(warehouseId, productId, quantity);
        defaultState();
        new MessageWindow("Message", "Changes are successfull!").Show();
    }
    private void addNewStock(int warehouseId, int productId, int quantity)
    {
        dbConnector.Stocks.Add
        (
            new DataBase.Entities.stock_entity
            {
                WarehouseId = warehouseId,
                ProductId = productId,
                Quantity = quantity
            }
        );
        dbConnector.SaveChanges();
    }
    private void updateStock(int warehouseId, int productId, int quantity)
    {
        DataBase.Entities.stock_entity Stock = dbConnector.Stocks.FirstOrDefault(w => w.WarehouseId == warehouseId && w.ProductId == productId)!;
        Stock.Quantity = quantity;
        dbConnector.SaveChanges();
    }
    private void warehouseManagementWarehouseStockCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWarehousesQuantityTextBox.Text = string.Empty;
        _warehouseManagementWarehouseWarehouseComboBox.Text = string.Empty;
        _warehouseManagementWarehouseProductComboBox.Text = string.Empty;
    }

}
