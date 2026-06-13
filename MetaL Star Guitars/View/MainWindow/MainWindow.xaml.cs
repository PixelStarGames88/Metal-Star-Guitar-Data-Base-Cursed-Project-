using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using MetaL_Star_Guitars.View.RegisterWindow;
using MetaL_Star_Guitars.View.RegisterWindow.LogisticWindow;
using System.Dynamic;
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

    // ==================== DELETE ====================

    private void deleteWarehouseEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int warehouseId = item.WarehouseId;
        var entity = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);
        if (entity != null) { dbConnector.Warehouses.Remove(entity); dbConnector.SaveChanges(); fill_WarehouseListBox(); }
    }
    private void deleteWriteOffEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int documentId = item.documentId;
        var entity = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (entity != null) { dbConnector.StockAdjustmentDocuments.Remove(entity); dbConnector.SaveChanges(); fill_WriteOffListBox(); }
    }

    // ==================== EDIT ====================

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
    private void editWriteOffEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int documentId = item.documentId;
        var entity = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (entity != null)
        {
            var w = new WriteOffToProductionRegister(dbConnector);
            w._warehouseManagementWriteOffToProductionDocumentIdLabel.Content = entity.StockAdjustmentDocumentId;
            w._warehouseManagementWriteOffToProductionTypeLabel.Content = entity.DocumentType;
            w._warehouseManagementWriteOffToProductionDateLabel.Content = entity.IssueDate;
            w._warehouseManagementWriteOffToProductionQuantityTextBox.Text = entity.Quantity.ToString();
            w._warehouseManagementWriteOffToProductionForOrderComboBox.SelectedValue = entity.ProductionOrderId;
            w._warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedValue = entity.WarehouseId;
            w._warehouseManagementWriteOffToProductionProductComboBox.SelectedValue = entity.ProductId;
            w.Show();
        }
    }


}