using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void productionStagesManagementCreateOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementReleaseProductsGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Visible;

        productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(sender, e);

        var productionStages = dbConnector.ProductionStages.ToList();
        _productionStagesManagementCreateOrderForStageComboBox.ItemsSource = productionStages;
        _productionStagesManagementCreateOrderForStageComboBox.DisplayMemberPath = "ProductionStageName";

        var product = dbConnector.Products.ToList();
        _productionStagesManagementCreateOrderProductComboBox.ItemsSource = product;
        _productionStagesManagementCreateOrderProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void productionStagesManagementCreateOrderProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string productname = null!;

        if (_productionStagesManagementCreateOrderProductComboBox.SelectedItem is product_entity selectedProduct)
            productname = selectedProduct.ProductName;

        var lastStock = dbConnector.Stocks.
            Join(
            dbConnector.Products, p => p.ProductId, s => s.ProductId,
            (p, s) => new
            {
                ProductName = s.ProductName,
                WarehouseId = p.WarehouseId,
                Quantity = p.Quantity
            }).
            Join(
            dbConnector.Warehouses, s => s.WarehouseId, w => w.WarehouseId,
            (s, w) => new
            {
                ProductName = s.ProductName,
                WarehouseName = w.WarehouseName,
                Quantity = s.Quantity
            }).
            Where(n => n.WarehouseName == "Кладовая предприятия (Свердловская область)"
            && n.ProductName == productname).Select(x => x.Quantity);
        int limit = lastStock?.FirstOrDefault() ?? 0;
        _productionStagesManagementCreateOrderLimitLabel.Content = ("< " + limit.ToString() + " pcs.");
    }
    private void productionStagesManagementCreateOrderToOrderButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        string productname = null!;
        string stagename = null!;

        if (_productionStagesManagementCreateOrderProductComboBox.SelectedItem is product_entity selectedProduct)
            productname = selectedProduct.ProductName;

        if (_productionStagesManagementCreateOrderForStageComboBox.SelectedItem is production_stage_entity selectedStage)
            stagename = selectedStage.ProductionStageName;

        dbConnector.ProductionOrders.Add(new production_order_entity
        {
            IssueDate = DateTime.UtcNow,
            Status = "Open",
            ProductId = dbConnector.Products.Where(n => n.ProductName == productname).OrderBy(n => n.ProductId).Select(n => n.ProductId)?.LastOrDefault() ?? throw new NullReferenceException(),
            ProductionStageId = dbConnector.ProductionStages.Where(n => n.ProductionStageName == stagename).OrderBy(n => n.ProductionStageId).Select(n => n.ProductionStageId)?.LastOrDefault() ?? throw new NullReferenceException(),
            Quantity = int.Parse(_productionStagesManagementCreateOrderQuantityTextBox.Text)
        });
        dbConnector.SaveChanges();
        productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(sender, e);
    }
    private void productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementCreateOrderForStageComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderProductComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderQuantityTextBox.Text = string.Empty;
        _productionStagesManagementCreateOrderLimitLabel.Content = ("< 0 pcs.");
        _productionStagesManagementCreateOrderDateLabel.Content = DateTime.Now.ToString("dd.MM.yyyy");
        int maxId = dbConnector.ProductionOrders.Max(p => (int?)p.ProductId) ?? 0;
        _productionStagesManagementCreateOrderDocumentIdLabel.Content = (maxId + 1).ToString();
    }

}
