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
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementWarehousesGrid.Visibility = Visibility.Visible;
        fill_StockListBox();
        fill_WarehouseListBox();
    }
    public void fill_WarehouseListBox()
    {
        var documents = dbConnector.Warehouses.ToList();

        _warehouseListBox.ItemsSource = documents;
    }
    private void editWarehouseEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int warehouseId = item.WarehouseId;
        var entity = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);
        if (entity != null)
        {
            var w = new WarehouseRegisterWindow(dbConnector);
            w._warehouseManagementWarehouseWarehouseIdLabel.Content = entity.WarehouseId;
            w._warehouseManagementWarehouseWarehouseNameTextBox.Text = entity.WarehouseName;
            w._warehouseManagementWarehouseCapacityTextBox.Text = entity.Capacity.ToString();
            w.Show();
        }
    }
    private void deleteWarehouseEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int warehouseId = item.WarehouseId;
        var entity = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);
        if (entity != null) { dbConnector.Warehouses.Remove(entity); dbConnector.SaveChanges(); fill_WarehouseListBox(); }
    }

    private void _createWarehouseButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new WarehouseRegisterWindow(dbConnector).Show();
    }
}