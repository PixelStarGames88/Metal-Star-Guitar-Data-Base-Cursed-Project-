using MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void productionStagesManagementProductionOrdersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Visible;
        fill_ProductionOrderListBox();
    }
    private void _createProductionOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ProductionOrderRegister(dbConnector).Show();
    }
    private void deleteProductionOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;
        var entity = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == orderId);
        if (entity != null) { dbConnector.ProductionOrders.Remove(entity); dbConnector.SaveChanges(); fill_ProductionOrderListBox(); }
    }
    public void fill_ProductionOrderListBox()
    {
        var orders = dbConnector.ProductionOrders
            .Join
            (
                dbConnector.Products, po => po.ProductId, p => p.ProductId,
                (po, p) => new
                {
                    order = po,
                    product = p
                }
            )
            .Join
            (
                dbConnector.ProductionStages, op => op.order.ProductionStageId, ps => ps.ProductionStageId,
                (op, ps) => new
                {
                    orderId = op.order.ProductionOrderId,
                    orderStatus = op.order.Status,
                    orderDate = op.order.IssueDate,
                    quantity = op.order.Quantity,
                    productName = op.product.ProductName,
                    stageName = ps.ProductionStageName
                }
            ).ToList();

        _productionOrderListBox.ItemsSource = orders;
    }
}
