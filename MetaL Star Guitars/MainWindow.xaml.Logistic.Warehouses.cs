using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementWarehousesButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Visible;
        defaultWindowState();
    }
    private void defaultWindowState()
    {
        _warehouseManagementWarehouseWarehouseNameTextBox.Text = string.Empty;
        _warehouseManagementWarehouseCapacityTextBox.Text = string.Empty;
        _warehouseManagementWarehousesQuantityTextBox.Text = string.Empty;
        _warehouseManagementWarehouseWarehouseComboBox.Text = string.Empty;
        _warehouseManagementWarehouseProductComboBox.Text = string.Empty;
        fill_warehouseManagementWarehouseWarehouseComboBox();
        fill_warehouseManagementWarehouseProductComboBox();
        fill_warehouseManagementWarehouseWarehousesListBox();
        fill_warehouseManagementWarehouseStockListBox();
        _warehouseManagementWarehouseWarehouseIdLabel.Content = ((dbConnector.Warehouses.Max(p => (int?)p.WarehouseId) ?? 0) + 1).ToString();
        _warehouseManagementWarehouseWarehouseRegisterButtonLabel.Content = "Register";
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
    private void fill_warehouseManagementWarehouseWarehousesListBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementWarehouseWarehousesListBox.ItemsSource = warehouses;
    }
    private void fill_warehouseManagementWarehouseStockListBox()
    {
        var stocks = dbConnector.Stocks
            .Join
            (
                dbConnector.Products, s => s.ProductId, p => p.ProductId,
                (s, p) => new 
                {
                    Stock = s,
                    Product = p
                }
            )
            .Join
            (
                dbConnector.Warehouses, s => s.Stock.WarehouseId, w => w.WarehouseId,
                (s, w) => new
                {
                    Quantity = s.Stock.Quantity,
                    WarehouseName = w.WarehouseName,
                    ProductName = s.Product.ProductName
                }
            ).ToList();
        _warehouseManagementWarehouseStockListBox.ItemsSource = stocks;
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
        defaultWindowState();
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
        fill_warehouseManagementWarehouseWarehousesListBox();
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
        defaultWindowState();
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
        fill_warehouseManagementWarehouseStockListBox();
    }
    private void DeleteWarehouseFromDB(object sender, MouseButtonEventArgs e)
    {
        dynamic selectedItem = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int warehouseId = selectedItem.WarehouseId;

        var warehouseToDelete = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);

        if (warehouseToDelete != null)
        {
            dbConnector.Warehouses.Remove(warehouseToDelete);
        }

        dbConnector.SaveChanges();

        fill_warehouseManagementWarehouseWarehousesListBox();
        warehouseManagementTransfersCancelButton_MouseDown(sender, e);
    }
    private void EditWarehouseInDB(object sender, MouseButtonEventArgs e)
    {
        dynamic selectedItem = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int warehouseId = selectedItem.WarehouseId;

        var warehouseToEdit = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);

        if (warehouseToEdit != null)
        {
            _warehouseManagementWarehouseWarehouseNameTextBox.Text = warehouseToEdit.WarehouseName;
            _warehouseManagementWarehouseCapacityTextBox.Text = warehouseToEdit.Capacity.ToString();
            _warehouseManagementWarehouseWarehouseIdLabel.Content = warehouseId;
        }
        _warehouseManagementWarehouseWarehouseRegisterButtonLabel.Content = "Save";
    }
    private void DeleteStockFromDB(object sender, MouseButtonEventArgs e)
    {
        dynamic selectedItem = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        string warehouseName = selectedItem.WarehouseName;
        string productName = selectedItem.ProductName;

        int warehouseId = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseName == warehouseName)!.WarehouseId;
        int productId = dbConnector.Products.FirstOrDefault(x => x.ProductName == productName)!.ProductId;
        var stockToDelete = dbConnector.Stocks.FirstOrDefault(x => x.WarehouseId == warehouseId && x.ProductId == productId);

        if (stockToDelete != null)
        {
            dbConnector.Stocks.Remove(stockToDelete);
        }

        dbConnector.SaveChanges();
        defaultWindowState();
        new MessageWindow("Message", "Changes are successfull!").Show();
    }
    private void EditStockInDB(object sender, MouseButtonEventArgs e)
    {
        dynamic selectedItem = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        string warehouseName = selectedItem.WarehouseName;
        string productName = selectedItem.ProductName;
        int quantity = selectedItem.Quantity;

        _warehouseManagementWarehousesQuantityTextBox.Text = quantity.ToString();
        _warehouseManagementWarehouseWarehouseComboBox.Text = warehouseName;
        _warehouseManagementWarehouseProductComboBox.Text = productName;

        _warehouseManagementWarehouseStockRegisterButtonLabel.Content = "Save";
    }
}