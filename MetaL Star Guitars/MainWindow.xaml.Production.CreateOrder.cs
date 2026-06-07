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
        fillForStageComboBoxProductionStagesManagement();
        fillProductComboBoxProductionStagesManagement();
    }
    private void fillForStageComboBoxProductionStagesManagement()
    {
        var productionStages = dbConnector.ProductionStages.ToList();
        _productionStagesManagementCreateOrderForStageComboBox.ItemsSource = productionStages;
        _productionStagesManagementCreateOrderForStageComboBox.DisplayMemberPath = "ProductionStageName";
    }
    private void fillProductComboBoxProductionStagesManagement()
    {
        var product = dbConnector.Products.ToList();
        _productionStagesManagementCreateOrderProductComboBox.ItemsSource = product;
        _productionStagesManagementCreateOrderProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void productionStagesManagementCreateOrderProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string productname = null!;

        if (_productionStagesManagementCreateOrderProductComboBox.SelectedItem is product_entity selectedProduct)
            productname = selectedProduct.ProductName;

        int limit = GetProductsCount(productname, "Кладовая предприятия (Свердловская область)");
    }
    private void productionStagesManagementCreateOrderToOrderButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        int productId = (_productionStagesManagementCreateOrderProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int stageId = (_productionStagesManagementCreateOrderForStageComboBox.SelectedItem as production_stage_entity)?.ProductionStageId ?? throw new NullReferenceException();

        int quantity = int.Parse(_productionStagesManagementCreateOrderQuantityTextBox.Text);

        string productName = (_productionStagesManagementCreateOrderProductComboBox.SelectedItem as product_entity)?.ProductName ?? throw new NullReferenceException();
        int orderId = int.Parse(_productionStagesManagementCreateOrderDocumentIdLabel.Content.ToString()!);

        if (!int.TryParse(_productionStagesManagementCreateOrderQuantityTextBox.Text, out quantity))
        {
            new MessageWindow("Error", "Enter integer number!").Show();
            return;
        }

        SaveOrUpdateProductionOrder(orderId, productId, stageId, quantity);

        productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(sender, e);

        dbConnector.SaveChanges();
    }
    private void productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementCreateOrderForStageComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderProductComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderQuantityTextBox.Text = string.Empty;
        _productionStagesManagementCreateOrderDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
        int maxId = dbConnector.ProductionOrders.Max(p => (int?)p.ProductId) ?? 0;
        _productionStagesManagementCreateOrderDocumentIdLabel.Content = (maxId + 1).ToString();
        _productionStagesManagementCreateOrderToOrderButtonLabel.Content = "To order";
        fill_productionOrdersListBox();
    }
    private int GetProductsCount(string productname, string warehousename)
    {
        var product = dbConnector.Stocks.
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
            Where(n => n.WarehouseName == warehousename
            && n.ProductName == productname).Select(x => x.Quantity);

        int productCount = product?.FirstOrDefault() ?? 0;

        return productCount;
    }
    private void fill_productionOrdersListBox()
    {
        var orders = dbConnector.ProductionOrders.ToList();
        _productionOrdersManagementOrdersListBox.ItemsSource = orders;
    }

    private void SaveOrUpdateProductionOrder(int orderId, int productId, int stageId, int quantity)
    {
        production_order_entity? order = GetProductionOrderById(orderId);

        if (order != null)
        {
            UpdateProductionOrder(order, productId, stageId, quantity);
            new MessageWindow("Message", "Production order\nupdated successfully!").Show();
        }
        else
        {
            CreateProductionOrderEntity(productId, stageId, quantity);
            new MessageWindow("Message", "Your production order\nwas made successfully!").Show();
        }

        dbConnector.SaveChanges();
    }

    private production_order_entity? GetProductionOrderById(int id)
    {
        return dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == id);
    }

    private void UpdateProductionOrder(production_order_entity order, int productId, int stageId, int quantity)
    {
        order.Quantity = quantity;
        order.ProductId = productId;
        order.ProductionStageId = stageId;
        order.Status = "Open";
        order.IssueDate = DateTime.UtcNow;
    }

    private void CreateProductionOrderEntity(int productId, int stageId, int quantity)
    {
        dbConnector.ProductionOrders.Add(new production_order_entity
        {
            IssueDate = DateTime.UtcNow,
            Status = "Open",
            Quantity = quantity,
            ProductId = productId,
            ProductionStageId = stageId
        });
    }
    private void EditProductionOrder_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is production_order_entity order)
        {
            _productionStagesManagementCreateOrderDocumentIdLabel.Content = order.ProductionOrderId.ToString();
            _productionStagesManagementCreateOrderDateLabel.Content = order.IssueDate.ToString("HH:mm dd.MM.yyyy");
            _productionStagesManagementCreateOrderQuantityTextBox.Text = order.Quantity.ToString();

            var product = dbConnector.Products.FirstOrDefault(p => p.ProductId == order.ProductId);
            if (product != null)
            {
                _productionStagesManagementCreateOrderProductComboBox.SelectedItem = product;
            }

            var stage = dbConnector.ProductionStages.FirstOrDefault(s => s.ProductionStageId == order.ProductionStageId);
            if (stage != null)
            {
                _productionStagesManagementCreateOrderForStageComboBox.SelectedItem = stage;
            }


            _productionStagesManagementCreateOrderToOrderButtonLabel.Content = "Save";
        }
    }

    private void DeleteProductionOrder_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is production_order_entity order)
        {
            var orderToDelete = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == order.ProductionOrderId);

            if (orderToDelete != null)
            {
                dbConnector.ProductionOrders.Remove(orderToDelete);
                dbConnector.SaveChanges();

                fill_productionOrdersListBox();
            }
        }
    }

}
