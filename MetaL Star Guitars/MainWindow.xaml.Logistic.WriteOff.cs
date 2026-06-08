using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementWriteOffToProductionButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
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
        _warehouseManagementWriteOffToProductionDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
        int maxId = dbConnector.StockAdjustmentDocuments.Max(p => (int?)p.StockAdjustmentDocumentId) ?? 0;
        _warehouseManagementWriteOffToProductionDocumentIdLabel.Content = (maxId + 1).ToString();
        _warehouseManagementWriteOffToProductionToWriteOffButtonLabel.Content = "To write-off";
        fill_writeOffDocumentsListBox();
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
        findLimit();
    }
    private void warehouseManagementWriteOffToProductionToWriteOffButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        int productId = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int warehouseId = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();

        dynamic? selectedOrder = _warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem;
        int orderId = selectedOrder.ProductionOrder.ProductionOrderId;

        int quantity = int.Parse(_warehouseManagementWriteOffToProductionQuantityTextBox.Text);

        string productName = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductName ?? throw new NullReferenceException();
        string warehouseName = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();

        int documentId = int.Parse(_warehouseManagementWriteOffToProductionDocumentIdLabel.Content.ToString()!);

        if (!ValidateQuantity(quantity, productName, warehouseName))
            return;

        SaveOrUpdateDocument(documentId, productId, warehouseId, orderId, quantity);

        warehouseManagementWriteOffToProductionCancelButtonLabel_MouseDown(sender, e);
    }

    private bool ValidateQuantity(int quantity, string productName, string warehouseName)
    {
        if (quantity > GetProductsCount(productName, warehouseName))
        {
            new MessageWindow("Error", "Enter number less than limit!").Show();
            return false;
        }

        return true;
    }

    private void SaveOrUpdateDocument(int documentId, int productId, int warehouseId, int orderId, int quantity)
    {
        stock_adjustment_document_entity? document = GetDocumentById(documentId);

        if (document != null)
        {
            UpdateDocument(document, productId, warehouseId, orderId, quantity);
            new MessageWindow("Message", "Document updated successfully!").Show();
        }
        else
        {
            CreateDocument(productId, warehouseId, orderId, quantity);
            new MessageWindow("Message", "Your document was made successfully!").Show();
        }

        dbConnector.SaveChanges();
    }

    private stock_adjustment_document_entity? GetDocumentById(int id)
    {
        return dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == id);
    }

    private void UpdateDocument(stock_adjustment_document_entity document, int productId, int warehouseId, int orderId, int quantity)
    {
        document.IssueDate = DateTime.UtcNow;
        document.DocumentType = "Decommissioning into production";
        document.Quantity = quantity;
        document.WarehouseId = warehouseId;
        document.ProductionOrderId = orderId;
        document.ProductId = productId;
    }

    private void CreateDocument(int productId, int warehouseId, int orderId, int quantity)
    {
        dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
        {
            IssueDate = DateTime.UtcNow,
            DocumentType = "Decommissioning into production",
            Quantity = quantity,
            WarehouseId = warehouseId,
            ProductionOrderId = orderId,
            ProductId = productId
        });
    }
    private void fill_writeOffDocumentsListBox()
    {
        var Documents = dbConnector.StockAdjustmentDocuments.ToList();
        _writeOffDocumentsListBox.ItemsSource = Documents;
    }
    private void EditDocument_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is stock_adjustment_document_entity document)
        {
            _warehouseManagementWriteOffToProductionDocumentIdLabel.Content = document.StockAdjustmentDocumentId.ToString();

            _warehouseManagementWriteOffToProductionDateLabel.Content = document.IssueDate.ToString("HH:mm dd.MM.yyyy");

            _warehouseManagementWriteOffToProductionQuantityTextBox.Text = document.Quantity.ToString();

            var warehouse = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseId == document.WarehouseId);

            if (warehouse != null)
            {
                _warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem =
                    warehouse;
            }

            var product = dbConnector.Products.FirstOrDefault(p => p.ProductId == document.ProductId);

            if (product != null)
            {
                _warehouseManagementWriteOffToProductionProductComboBox.SelectedItem = product;
            }

            var orderItem = _warehouseManagementWriteOffToProductionForOrderComboBox
            .Items.Cast<dynamic>().FirstOrDefault(x => x.ProductionOrder.ProductionOrderId == document.ProductionOrderId);

            if (orderItem != null)
            {
                _warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem = orderItem;
            }

            findLimit();

            _warehouseManagementWriteOffToProductionToWriteOffButtonLabel.Content = "Save";
        }
    }
    private void DeleteDocument_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock textBlock &&
            textBlock.DataContext is stock_adjustment_document_entity document)
        {
            var documentToDelete = dbConnector.StockAdjustmentDocuments
                .FirstOrDefault(x => x.StockAdjustmentDocumentId == document.StockAdjustmentDocumentId);

            if (documentToDelete != null)
            {
                dbConnector.StockAdjustmentDocuments.Remove(documentToDelete);
                dbConnector.SaveChanges();

                fill_writeOffDocumentsListBox();
            }
        }
    }
    private void findLimit()
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
}