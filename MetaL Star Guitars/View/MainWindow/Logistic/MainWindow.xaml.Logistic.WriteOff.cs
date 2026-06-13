using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetaL_Star_Guitars.View.RegisterWindow;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementWriteOffToProductionButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Visible;

        fill_WriteOffListBox();
    }
    private void fill_WriteOffListBox()
    {
        var documents = dbConnector.StockAdjustmentDocuments
            .Join
            (
                dbConnector.Products, sad => sad.ProductId, p => p.ProductId,
                (sad, p) => new
                {
                    document = sad,
                    product = p
                }
            )
            .Join
            (
                dbConnector.Warehouses, sad => sad.document.WarehouseId, w => w.WarehouseId,
                (sad, w) => new
                {
                    document = sad.document,
                    product = sad.product,
                    warehouse = w
                }
            )
            .Join
            (
                dbConnector.ProductionOrders, sad => sad.document.ProductionOrderId, po => po.ProductionStageId,
                (sad, po) => new
                {
                    document = sad.document,
                    product = sad.product,
                    warehouse = sad.warehouse,
                    productionOrder = po
                }
            )
            .Join
            (
                dbConnector.ProductionStages, sad => sad.productionOrder.ProductionStageId, ps => ps.ProductionStageId,
                (sad, ps) => new
                {
                    documentId = sad.document.StockAdjustmentDocumentId,
                    documentType = sad.document.DocumentType,
                    documentDate = sad.document.IssueDate,
                    documentQuantity = sad.document.Quantity,
                    product = sad.product.ProductName,
                    warehouse = sad.warehouse.WarehouseName,
                    productionStage = ps.ProductionStageName
                }
            ).ToList();

        _writeOffListBox.ItemsSource = documents;
    }
    private void _createWriteOffButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new WriteOffToProductionRegister(dbConnector).Show();
    }
}