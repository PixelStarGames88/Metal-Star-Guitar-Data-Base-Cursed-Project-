using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;
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



    private void productionStagesManagementProductsButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Visible;
        fill_ProductListBox();
    }

    private void productionStagesManagementReleaseProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Visible;
        fill_ReleaseProductListBox();
    }

    private void productionStagesManagementProductionOrdersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Visible;
        fill_ProductionOrderListBox();
    }
    

    private void fill_ProductListBox()
    {
        var products = dbConnector.Products
            .Select
            (
                p => new
                {
                    productId = p.ProductId,
                    productName = p.ProductName,
                    productPrice = p.Price
                }
            ).ToList();

        _productListBox.ItemsSource = products;
    }

    private void fill_ReleaseProductListBox()
    {
        var documents = dbConnector.StockAdjustmentDocuments
            .Join
            (
                dbConnector.Products, d => d.ProductId, p => p.ProductId,
                (d, p) => new
                {
                    doc = d,
                    product = p
                }
            )
            .Join
            (
                dbConnector.Warehouses, dp => dp.doc.WarehouseId, w => w.WarehouseId,
                (dp, w) => new
                {
                    orderId = dp.doc.StockAdjustmentDocumentId,
                    orderStartData = dp.doc.IssueDate,
                    orderStatus = dp.doc.DocumentType,
                    orderFinishData = dp.doc.IssueDate,
                    senderWarehouse = w.WarehouseName,
                    recipientWarehouse = dp.product.ProductName
                }
            ).ToList();

        _releaseProductListBox.ItemsSource = documents;
    }

    private void fill_ProductionOrderListBox()
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

    // ==================== CREATE BUTTONS ====================



    private void _createProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ProductRegisterWindow(dbConnector).Show();
    }

    private void _createReleaseProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ReleaseProductOrderRegisterWindow(dbConnector).Show();
    }

    private void _createProductionOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ProductionOrderRegister(dbConnector).Show();
    }

    // ==================== STAGE DELETE/EDIT ====================

    

    

    // ==================== PRODUCT DELETE/EDIT ====================

    private void deleteProductEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int productId = item.productId;
        var entity = dbConnector.Products.FirstOrDefault(x => x.ProductId == productId);
        if (entity != null) { dbConnector.Products.Remove(entity); dbConnector.SaveChanges(); fill_ProductListBox(); }
    }

    private void editProductEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int productId = item.productId;
        var entity = dbConnector.Products.FirstOrDefault(x => x.ProductId == productId);

        if (entity != null)
        {
            var w = new ProductRegisterWindow(dbConnector);
            w._productionStagesManagementProductProductIdLabel.Content = entity.ProductId;
            w._productionStagesManagementProductsProductsNameTextBox.Text = entity.ProductName;
            w._productionStagesManagementProductsPriceTextBox.Text = entity.Price.ToString();
            w._productionStagesManagementProductRegisterButtonLabel.Content = "Save";
            w.Show();
        }
    }

    // ==================== RELEASE PRODUCT DELETE/EDIT ====================

    private void deleteReleaseProductEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int documentId = item.orderId;
        var entity = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);
        if (entity != null) { dbConnector.StockAdjustmentDocuments.Remove(entity); dbConnector.SaveChanges(); fill_ReleaseProductListBox(); }
    }

    private void editReleaseProductEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int documentId = item.orderId;
        var entity = dbConnector.StockAdjustmentDocuments.FirstOrDefault(x => x.StockAdjustmentDocumentId == documentId);

        if (entity != null)
        {
            var w = new ReleaseProductOrderRegisterWindow(dbConnector);

            w._productionStagesManagementReleaseProductsDocumentIdLabel.Content = entity.StockAdjustmentDocumentId;
            w._productionStagesManagementReleaseProductsDateLabel.Content = entity.IssueDate.ToString("HH\\:mm dd.MM.yyyy");
            w._productionStagesManagementReleaseProductsQuantityTextBox.Text = entity.Quantity.ToString();

            w._productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem =
                dbConnector.ProductionOrders.FirstOrDefault(po => po.ProductionOrderId == entity.ProductionOrderId);
            w._productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem =
                dbConnector.Warehouses.FirstOrDefault(wh => wh.WarehouseId == entity.WarehouseId);
            w._productionStagesManagementReleaseProductsProductComboBox.SelectedItem =
                dbConnector.Products.FirstOrDefault(p => p.ProductId == entity.ProductId);

            foreach (ComboBoxItem typeItem in w._productionStagesManagementReleaseProductsTypeComboBox.Items)
            {
                if (typeItem.Content.ToString() == entity.DocumentType)
                {
                    w._productionStagesManagementReleaseProductsTypeComboBox.SelectedItem = typeItem;
                    break;
                }
            }

            w._productionStagesManagementReleaseProductsToReleaseButtonLabel.Content = "Save";
            w.Show();
        }
    }

    // ==================== PRODUCTION ORDER DELETE/EDIT ====================

    private void deleteProductionOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;
        var entity = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == orderId);
        if (entity != null) { dbConnector.ProductionOrders.Remove(entity); dbConnector.SaveChanges(); fill_ProductionOrderListBox(); }
    }

    private void editProductionOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;
        var entity = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == orderId);

        if (entity != null)
        {
            var w = new ProductionOrderRegister(dbConnector);

            w._productionStagesManagementCreateOrderDocumentIdLabel.Content = entity.ProductionOrderId;
            w._productionStagesManagementCreateOrderStatusLabel.Content = entity.Status;
            w._productionStagesManagementCreateOrderDateLabel.Content = entity.IssueDate.ToString("dd.MM.yyyy");
            w._productionStagesManagementCreateOrderQuantityTextBox.Text = entity.Quantity.ToString();

            w._productionStagesManagementCreateOrderForStageComboBox.SelectedItem =
                dbConnector.ProductionStages.FirstOrDefault(ps => ps.ProductionStageId == entity.ProductionStageId);
            w._productionStagesManagementCreateOrderProductComboBox.SelectedItem =
                dbConnector.Products.FirstOrDefault(p => p.ProductId == entity.ProductId);

            w._productionStagesManagementCreateOrderToOrderButtonLabel.Content = "Save";
            w.Show();
        }
    }
}