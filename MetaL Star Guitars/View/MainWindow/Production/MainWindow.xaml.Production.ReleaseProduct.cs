using MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void productionStagesManagementReleaseProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Visible;
        fill_ReleaseProductListBox();
    }

    private void _createReleaseProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ReleaseProductOrderRegisterWindow(dbConnector).Show();
    }
    private void deleteReleaseProductEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int documentId = item.orderId;
        var entity = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (entity != null) { dbConnector.StockAdjustmentDocuments.Remove(entity); dbConnector.SaveChanges(); fill_ReleaseProductListBox(); }
    }
    public void fill_ReleaseProductListBox()
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
                dbConnector.ProductionOrders, sad => sad.document.ProductionOrderId, po => po.ProductionOrderId,
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

        _releaseProductListBox.ItemsSource = documents;
    }
}