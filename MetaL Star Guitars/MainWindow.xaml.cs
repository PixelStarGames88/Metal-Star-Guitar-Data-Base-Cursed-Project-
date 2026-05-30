using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    DataBaseConnector dbConnector;
    public MainWindow()
    {
        InitializeComponent();
        dbConnector = new DataBaseConnector();
    }
    private void someButton_MouseEnterYellow(object sender, MouseEventArgs e)
    {
        if (sender is Label label) label.Foreground = Brushes.Yellow;
    }
    private void someButton_MouseLeaveWhite(object sender, MouseEventArgs e)
    {
        if (sender is Label label) label.Foreground = Brushes.White;
    }

    private void authorizationEnterButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_authorizationPostComboBox.Text == "Warehouse manager")
        {
            _authorizationGrid.Visibility = Visibility.Collapsed;
            _warehouseManagementGrid.Visibility = Visibility.Visible;
            warehouseManagementWriteOffToProductionButton_MouseDown(sender, e);
        }
        else if (_authorizationPostComboBox.Text == "Production stages manager")
        {
            _authorizationGrid.Visibility = Visibility.Collapsed;
            _productionStagesManagementGrid.Visibility = Visibility.Visible;
            productionStagesManagementReleaseProductsButton_MouseDown(sender, e);
        }
    }
    private void warehouseManagementExitToMenuButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _authorizationGrid.Visibility = Visibility.Visible;
        _warehouseManagementGrid.Visibility = Visibility.Collapsed;
    }

    private void warehouseManagementWriteOffToProductionButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Visible;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        fillForOrderComboBox();
        fillFromWarehouseComboBox();
        //fillProductComboBox();
    }
    private void fillForOrderComboBox()
    {
        var orders = dbConnector.ProductionOrders.ToList();
        _warehouseManagementWriteOffToProductionForOrderComboBox.ItemsSource = orders;
        _warehouseManagementWriteOffToProductionForOrderComboBox.DisplayMemberPath = "";
    }
    private void fillFromWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void fillProductComboBox()
    {
        var products = dbConnector.Products.
            Join(dbConnector.Stocks,
            s => s.ProductId, p => p.ProductId,
            (p, s) => new
            {
                ProductName = p.ProductName,
                ProductId = s.ProductId,
                WarehouseId = s.WarehouseId
            }).Join(dbConnector.Warehouses,
            w => w.WarehouseId, s => s.WarehouseId,
            (w, s) => new
            {
                ProductName = w.ProductName,
                ProductId = w.ProductId,
                WarehouseName = s.WarehouseName
            }).Where(w => w.WarehouseName == _warehouseManagementWriteOffToProductionFromWarehouseComboBox.Text).ToList();

        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.ItemsSource = products;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.DisplayMemberPath = "ProductName";
    }
    private void warehouseManagementTransfersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Visible;
    }

    private void productionStagesManagementExitToMenuButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementGrid.Visibility = Visibility.Collapsed;
        _authorizationGrid.Visibility = Visibility.Visible;
    }

    private void productionStagesManagementReleaseProductsButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementReleaseProductsGrid.Visibility = Visibility.Visible;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
    }
}