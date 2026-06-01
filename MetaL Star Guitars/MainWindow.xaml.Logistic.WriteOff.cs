using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementWriteOffToProductionButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Visible;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        warehouseManagementWriteOffToProductionCancelButtonLabel_MouseDown(sender, e);
        fillForOrderComboBoxWarehouseManagement();
        fillFromWarehouseComboBoxWarehouseManagement();
    }
    private void fillForOrderComboBoxWarehouseManagement()
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
        _warehouseManagementWriteOffToProductionForOrderComboBox.ItemsSource = orders;
        _warehouseManagementWriteOffToProductionForOrderComboBox.DisplayMemberPath = "OrderDescription";
    }
    private void fillFromWarehouseComboBoxWarehouseManagement()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void warehouseManagementWriteOffToProductionCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWriteOffToProductionForOrderComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionProductComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionQuantityTextBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionLimitLabel.Content = (" < 0 pcs.");
        _warehouseManagementWriteOffToProductionDateLabel.Content = DateTime.Now.ToString("dd.MM.yyyy");
        int maxId = dbConnector.StockAdjustmentDocuments.Max(p => (int?)p.ProductId) ?? 0;
        _warehouseManagementWriteOffToProductionDocumentIdLabel.Content = (maxId + 1).ToString();
    }
    private void warehouseManagementWriteOffToProductionForOrderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string warehousename = null!;

        if (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem is warehouse_entity selectedWarehouse)
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

        _warehouseManagementWriteOffToProductionProductComboBox.ItemsSource = products;
        _warehouseManagementWriteOffToProductionProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void warehouseManagementWriteOffToProductionProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string? productname = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductName;

        string? warehousename = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName;

        var lastStock = dbConnector.Stocks.
            Join(
            dbConnector.Products, s => s.ProductId, p => p.ProductId,
            (s, p) => new
            {
                ProductName = p.ProductName,
                WarehouseId = s.WarehouseId,
                Quantity = s.Quantity
            }).
            Join(
            dbConnector.Warehouses, s => s.WarehouseId, w => w.WarehouseId,
            (s, w) => new
            {
                ProductName = s.ProductName,
                WarehouseName = w.WarehouseName,
                Quantity = s.Quantity
            }).
            Where(n => n.WarehouseName == warehousename
            && n.ProductName == productname).Select(x => x.Quantity);

        int limit = lastStock?.FirstOrDefault() ?? 0;
        _warehouseManagementWriteOffToProductionLimitLabel.Content = (" < " + limit.ToString() + " pcs.");
    }
    private void warehouseManagementWriteOffToProductionToWriteOffButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        int productid = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int warehouseid = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        dynamic? selectedItem = _warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem;
        int orderid = selectedItem.ProductionOrder.ProductionOrderId;
        int quantity;
        if(!int.TryParse(_warehouseManagementWriteOffToProductionQuantityTextBox.Text, out quantity))
        {
            new MessageWindow("Error", "Enter integer number!").Show();
            return;
        }
        string? productname = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductName ?? throw new NullReferenceException();
        string? warehousename = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();

        if (quantity > GetProductsCount(productname, warehousename))
        {
            new MessageWindow("Error", "Enter number less than limit!").Show();
            return;
        }
        dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
        {
            IssueDate = DateTime.UtcNow,
            DocumentType = "Decommissioning into production",
            Quantity = quantity,
            WarehouseId = warehouseid,
            ProductionOrderId = orderid,
            ProductId = productid
        });
        dbConnector.SaveChanges();
        warehouseManagementWriteOffToProductionCancelButtonLabel_MouseDown(sender, e);
        new MessageWindow("Message", "Your document was made successfully!").Show();
    }
}