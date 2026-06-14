using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow;

public partial class WriteOffToProductionRegister : Window
{
    private DataBaseConnector dbConnector;
    public WriteOffToProductionRegister(DataBaseConnector dateBaseConnector)
    {
        InitializeComponent();
        dbConnector = dateBaseConnector;
        defaultState();
    }
    private void someButton_MouseEnterYellow(object sender, MouseEventArgs e)
    {
        if (sender is Label label) label.Foreground = Brushes.Yellow;
    }
    private void someButton_MouseLeaveWhite(object sender, MouseEventArgs e)
    {
        if (sender is Label label) label.Foreground = Brushes.White;
    }
    private void defaultState()
    {
        _warehouseManagementWriteOffToProductionForOrderComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionProductComboBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionQuantityTextBox.Text = string.Empty;
        _warehouseManagementWriteOffToProductionLimitLabel.Content = "< 0 pcs.";
        _warehouseManagementWriteOffToProductionDocumentIdLabel.Content = ((dbConnector.StockAdjustmentDocuments.Max(d => (int?)d.StockAdjustmentDocumentId) ?? 0) + 1).ToString();
        _warehouseManagementWriteOffToProductionTypeLabel.Content = "Write-off to production";
        _warehouseManagementWriteOffToProductionDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
        _warehouseManagementWriteOffToProductionToWriteOffButtonLabel.Content = "To write-off";
        fill_forOrderComboBox();
        fill_fromWarehouseComboBox();
    }
    private void fill_forOrderComboBox()
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
                dbConnector.ProductionStages, po => po.order.ProductionStageId, s => s.ProductionStageId,
                (po, s) => new
                {
                    ProductionOrderId = po.order.ProductionOrderId,
                    product = po.product,
                    stage = s,
                    description = (po.order.Quantity + " " + po.product.ProductName + " for " + s.ProductionStageName)
                }
            ).ToList();
        _warehouseManagementWriteOffToProductionForOrderComboBox.ItemsSource = orders;
        _warehouseManagementWriteOffToProductionForOrderComboBox.DisplayMemberPath = "description";
    }
    private void fill_fromWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementWriteOffToProductionFromWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void fill_productComboBox()
    {
        if (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem is not warehouse_entity warehouse)
            return;

        var products = dbConnector.Stocks
            .Join
            (
                dbConnector.Products, s => s.ProductId, p => p.ProductId,
                (s, p) => new
                {
                    Stock = s,
                    Product = p
                }
            )
            .Where(x => x.Stock.WarehouseId == warehouse.WarehouseId)
            .Select(x => x.Product)
            .Distinct()
            .ToList();

        _warehouseManagementWriteOffToProductionProductComboBox.ItemsSource = products;
        _warehouseManagementWriteOffToProductionProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void updateLimitLabel()
    {
        if (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem is not warehouse_entity warehouse)
            return;
        if (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem is not product_entity product)
            return;

        int quantity = dbConnector.Stocks
            .Where(s => s.WarehouseId == warehouse.WarehouseId && s.ProductId == product.ProductId)
            .Select(s => s.Quantity)
            .FirstOrDefault();

        _warehouseManagementWriteOffToProductionLimitLabel.Content = "< " + quantity + " pcs.";
    }
    private void warehouseManagementWriteOffToProductionForOrderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        fill_productComboBox();
    }
    private void warehouseManagementWriteOffToProductionProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        updateLimitLabel();
    }
    private void warehouseManagementWriteOffToProductionCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        defaultState();
    }
    private void warehouseManagementWriteOffToProductionToWriteOffButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice production order!").Show();
            return;
        }
        if (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice warehouse!").Show();
            return;
        }
        if (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice product!").Show();
            return;
        }
        if (!int.TryParse(_warehouseManagementWriteOffToProductionQuantityTextBox.Text.Trim(), out int quantity) || quantity <= 0)
        {
            new MessageWindow("Error", "Enter valid quantity!").Show();
            return;
        }



        int warehouseId = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        int productId = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();

        int stockQuantity = dbConnector.Stocks
            .Where(s => s.WarehouseId == warehouseId && s.ProductId == productId)
            .Select(s => s.Quantity)
            .FirstOrDefault();

        if (quantity > stockQuantity)
        {
            new MessageWindow("Error", "Quantity exceeds stock limit!").Show();
            return;
        }

        int documentId = int.Parse(_warehouseManagementWriteOffToProductionDocumentIdLabel.Content.ToString()!);

        if (_warehouseManagementWriteOffToProductionToWriteOffButtonLabel.Content.ToString() == "Save")
            update_WriteOff(documentId);
        else
            add_newWriteOff(documentId);

        this.Close();
    }
    private void add_newWriteOff(int documentId)
    {
        dynamic productionOrder = _warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem ?? throw new NullReferenceException();
        int productionOrderId = productionOrder.ProductionOrderId ?? throw new NullReferenceException();
        int warehouseId = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        int productId = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_warehouseManagementWriteOffToProductionQuantityTextBox.Text.Trim());

        dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
        {
            StockAdjustmentDocumentId = documentId,
            IssueDate = DateTime.UtcNow,
            DocumentType = "Decommissioning into production",
            Quantity = quantity,
            WarehouseId = warehouseId,
            ProductId = productId,
            ProductionOrderId = productionOrderId
        });

        dbConnector.SaveChanges();

        updateStockAfterWriteOff(documentId);
        updateProductionOrderStatusToCompleted(productionOrderId);

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_WriteOffListBox();
        mainWindow?.fill_ProductionOrderListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
    }
    private void update_WriteOff(int documentId)
    {
        int productionOrderId = (_warehouseManagementWriteOffToProductionForOrderComboBox.SelectedItem as production_order_entity)?.ProductionOrderId ?? throw new NullReferenceException();
        int warehouseId = (_warehouseManagementWriteOffToProductionFromWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        int productId = (_warehouseManagementWriteOffToProductionProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_warehouseManagementWriteOffToProductionQuantityTextBox.Text.Trim());

        var existingDoc = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (existingDoc != null)
        {
            existingDoc.Quantity = quantity;
            existingDoc.WarehouseId = warehouseId;
            existingDoc.ProductId = productId;
            existingDoc.ProductionOrderId = productionOrderId;
        }

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_WriteOffListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
    private void updateStockAfterWriteOff(int documentId)
    {
        var document = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (document == null)
        {
            new MessageWindow("Error", "Document not found!").Show();
            return;
        }

        var stock = dbConnector.Stocks.FirstOrDefault(s =>
            s.WarehouseId == document.WarehouseId &&
            s.ProductId == document.ProductId);

        if (stock != null)
        {
            stock.Quantity -= document.Quantity;
            if (stock.Quantity < 0) stock.Quantity = 0;
            dbConnector.SaveChanges();
        }
        else
        {
            new MessageWindow("Error", "Stock not found for this product and warehouse!").Show();
        }
    }
    private void updateProductionOrderStatusToCompleted(int productionOrderId)
    {
        var order = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == productionOrderId);
        if (order != null)
        {
            order.Status = "Closed";
            dbConnector.SaveChanges();
        }
        else
        {
            new MessageWindow("Error", "Production order not found!").Show();
        }
    }
}