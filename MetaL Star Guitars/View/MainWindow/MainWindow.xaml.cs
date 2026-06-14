using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;
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
            productionStagesManagementProductionOrdersButton_MouseDown(sender, e);
        }
    }
    private void warehouseManagementExitToMenuButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _authorizationGrid.Visibility = Visibility.Visible;
        _warehouseManagementGrid.Visibility = Visibility.Collapsed;
    }

    private void productionStagesManagementExitToMenuButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementGrid.Visibility = Visibility.Collapsed;
        _authorizationGrid.Visibility = Visibility.Visible;
    }

    private void warehouseManagementTransactionButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementTransactionGrid.Visibility = Visibility.Visible;

        fill_warehouseManagementTransfersTransactionListBox();
    }
    private void fill_warehouseManagementTransfersTransactionListBox()
    {
        var transactions = dbConnector.TransferTransactions
            .Join
            (
                dbConnector.Warehouses, t => t.WarehouseId, w => w.WarehouseId,
                (t, w) => new
                {
                    IssueDate = t.IssueDate,
                    TransferTransactionId = t.TransferTransactionId,
                    TransferOrderId = t.TransferOrderId,
                    WarehouseName = w.WarehouseName,
                    TransactionType = t.TransactionType
                }
            ).ToList();
        _warehouseManagementTransfersTransactionListBox.ItemsSource = transactions;
    }
}