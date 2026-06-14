using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;

public partial class ReleaseProductOrderRegisterWindow : Window
{
    private DataBaseConnector dbConnector;
    public ReleaseProductOrderRegisterWindow(DataBaseConnector dateBaseConnector)
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
        _productionStagesManagementReleaseProductsToOrderComboBox.Text = string.Empty;
        _productionStagesManagementReleaseProductsToWarehouseComboBox.Text = string.Empty;
        _productionStagesManagementReleaseProductsProductComboBox.Text = string.Empty;
        _productionStagesManagementReleaseProductsQuantityTextBox.Text = string.Empty;
        _productionStagesManagementReleaseProductsTypeComboBox.SelectedIndex = 0;
        _productionStagesManagementReleaseProductsDocumentIdLabel.Content = ((dbConnector.StockAdjustmentDocuments.Max(d => (int?)d.StockAdjustmentDocumentId) ?? 0) + 1).ToString();
        _productionStagesManagementReleaseProductsDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
        _productionStagesManagementReleaseProductsToReleaseButtonLabel.Content = "To release";
        fill_toOrderComboBox();
        fill_toWarehouseComboBox();
        fill_productComboBox();
    }
    private void fill_toOrderComboBox()
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
        _productionStagesManagementReleaseProductsToOrderComboBox.ItemsSource = orders;
        _productionStagesManagementReleaseProductsToOrderComboBox.DisplayMemberPath = "description";
    }
    private void fill_toWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _productionStagesManagementReleaseProductsToWarehouseComboBox.ItemsSource = warehouses;
        _productionStagesManagementReleaseProductsToWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void fill_productComboBox()
    {
        var products = dbConnector.Products.ToList();
        _productionStagesManagementReleaseProductsProductComboBox.ItemsSource = products;
        _productionStagesManagementReleaseProductsProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        defaultState();
    }
    private void productionStagesManagementReleaseProductsToReleaseButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice production order!").Show();
            return;
        }
        if (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice warehouse!").Show();
            return;
        }
        if (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice product!").Show();
            return;
        }
        if (!int.TryParse(_productionStagesManagementReleaseProductsQuantityTextBox.Text.Trim(), out int quantity) || quantity <= 0)
        {
            new MessageWindow("Error", "Enter valid quantity!").Show();
            return;
        }
        if (_productionStagesManagementReleaseProductsTypeComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice document type!").Show();
            return;
        }

        int documentId = int.Parse(_productionStagesManagementReleaseProductsDocumentIdLabel.Content.ToString()!);

        if (_productionStagesManagementReleaseProductsToReleaseButtonLabel.Content.ToString() == "Save")
            update_ReleaseProduct(documentId);
        else
            add_newReleaseProduct(documentId);

        this.Close();
    }
    private void add_newReleaseProduct(int documentId)
    {
        dynamic productionOrder = _productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem;
        int productionOrderId = productionOrder.ProductionOrderId ?? throw new NullReferenceException();
        int warehouseId = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        int productId = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_productionStagesManagementReleaseProductsQuantityTextBox.Text.Trim());
        string documentType = (_productionStagesManagementReleaseProductsTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? throw new NullReferenceException();

        dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
        {
            StockAdjustmentDocumentId = documentId,
            IssueDate = DateTime.UtcNow,
            DocumentType = documentType,
            Quantity = quantity,
            WarehouseId = warehouseId,
            ProductId = productId,
            ProductionOrderId = productionOrderId
        });

        dbConnector.SaveChanges();

        updateStockAfterRelease(documentId);
        updateProductionOrderStatusToCompleted(productionOrderId);

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_ReleaseProductListBox();
        mainWindow?.fill_ProductionOrderListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        this.Close();
    }
    private void update_ReleaseProduct(int documentId)
    {
        int productionOrderId = (_productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem as production_order_entity)?.ProductionOrderId ?? throw new NullReferenceException();
        int warehouseId = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
        int productId = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_productionStagesManagementReleaseProductsQuantityTextBox.Text.Trim());
        string documentType = (_productionStagesManagementReleaseProductsTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? throw new NullReferenceException();

        var existingDoc = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (existingDoc != null)
        {
            existingDoc.DocumentType = documentType;
            existingDoc.Quantity = quantity;
            existingDoc.WarehouseId = warehouseId;
            existingDoc.ProductId = productId;
            existingDoc.ProductionOrderId = productionOrderId;
        }

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_ReleaseProductListBox();
        updateProductionOrderStatusToCompleted(productionOrderId);
        updateStockAfterRelease(documentId);
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
    private void updateStockAfterRelease(int documentId)
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
            stock.Quantity += document.Quantity;
            dbConnector.SaveChanges();
        }
        else
        {
            dbConnector.Stocks.Add(new stock_entity
            {
                WarehouseId = document.WarehouseId,
                ProductId = document.ProductId,
                Quantity = document.Quantity
            });
            dbConnector.SaveChanges();
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