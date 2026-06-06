using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars
{
    public partial class MainWindow : Window
    {
        private void productionStagesManagementReleaseProductsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _productionStagesManagementReleaseProductsGrid.Visibility = Visibility.Visible;
            _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
            productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(sender, e);
            fillForOrderComboBoxProductionStagesManagement();
            fillFromWarehouseComboBoxProductionStagesManagement();
        }
        private void fillFromWarehouseComboBoxProductionStagesManagement()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _productionStagesManagementReleaseProductsToWarehouseComboBox.ItemsSource = warehouses;
            _productionStagesManagementReleaseProductsToWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }
        private void fillForOrderComboBoxProductionStagesManagement()
        {
            var orders = dbConnector.ProductionOrders.
            Join(dbConnector.Products,
            o => o.ProductId, p => p.ProductId,
            (o, p) => new
            {
                Product = p,
                ProductionOrder = o
            }).Join(dbConnector.ProductionStages,
            o => o.ProductionOrder.ProductionStageId, ps => ps.ProductionStageId,
            (o, ps) => new
            {
                ProductionOrder = o.ProductionOrder,
                OrderDescription = (o.Product.ProductName + " | " + ps.ProductionStageName + " | " + o.ProductionOrder.Quantity)
            }).ToList();
            _productionStagesManagementReleaseProductsToOrderComboBox.ItemsSource = orders;
            _productionStagesManagementReleaseProductsToOrderComboBox.DisplayMemberPath = "OrderDescription";
        }
        private void productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _productionStagesManagementReleaseProductsToOrderComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsToWarehouseComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsProductComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsQuantityTextBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsTypeComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsLimitLabel.Content = (" < 0 pcs.");
            _productionStagesManagementReleaseProductsDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
            int maxId = dbConnector.StockAdjustmentDocuments.Max(p => (int?)p.ProductId) ?? 0;
            _productionStagesManagementReleaseProductsDocumentIdLabel.Content = (maxId + 1).ToString();
        }
        private void productionStagesManagementReleaseProductsToWarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string warehousename = null!;

            if (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem is warehouse_entity selectedWarehouse)
                warehousename = selectedWarehouse.WarehouseName;

            var products = dbConnector.Products.
            Join(dbConnector.Stocks,
                p => p.ProductId, s => s.ProductId,
                (p, s) => new { Product = p, Stock = s }).
            Join(dbConnector.Warehouses,
                combined => combined.Stock.WarehouseId,
                w => w.WarehouseId,
                (combined, w) =>
                new { combined.Product, Warehouse = w }).
            Where(x => x.Warehouse.WarehouseName == warehousename)
            .Select(x => x.Product).Distinct().ToList();

            _productionStagesManagementReleaseProductsProductComboBox.ItemsSource = products;
            _productionStagesManagementReleaseProductsProductComboBox.DisplayMemberPath = "ProductName";
        }
        private void productionStagesManagementReleaseProductsProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string productname = null!;

            if (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem is product_entity selectedProduct)
                productname = selectedProduct.ProductName;

            int limit = GetProductsCount(productname, "Кладовая предприятия (Свердловская область)");
            _productionStagesManagementReleaseProductsLimitLabel.Content = (" < " + limit.ToString() + " pcs.");
        }
        private void productionStagesManagementReleaseProductsToReleaseButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int productid = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
            int warehouseid = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            dynamic? selectedItem = _productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem;
            int orderid = selectedItem.ProductionOrder.ProductionOrderId;
            int quantity;
            if (!int.TryParse(_productionStagesManagementReleaseProductsQuantityTextBox.Text, out quantity))
            {
                new MessageWindow("Error", "Enter integer number!").Show();
                return;
            }
            string? productname = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductName ?? throw new NullReferenceException();
            string? warehousename = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();

            if (quantity > GetProductsCount(productname, warehousename))
            {
                new MessageWindow("Error", "Enter number less than limit!").Show();
                return;
            }
            dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
            {
                IssueDate = DateTime.UtcNow,
                DocumentType = _productionStagesManagementReleaseProductsTypeComboBox.Text,
                Quantity = quantity,
                WarehouseId = warehouseid,
                ProductionOrderId = orderid,
                ProductId = productid
            });
            dbConnector.SaveChanges();
            productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(sender, e);
            new MessageWindow("Message", "Your document was made successfully!").Show();
        }
    }
}
