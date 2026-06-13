using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;

public partial class ProductionOrderRegister : Window
{
    private DataBaseConnector dbConnector;
    public ProductionOrderRegister(DataBaseConnector dateBaseConnector)
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
        _productionStagesManagementCreateOrderForStageComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderProductComboBox.Text = string.Empty;
        _productionStagesManagementCreateOrderQuantityTextBox.Text = string.Empty;
        _productionStagesManagementCreateOrderDocumentIdLabel.Content = ((dbConnector.ProductionOrders.Max(po => (int?)po.ProductionOrderId) ?? 0) + 1).ToString();
        _productionStagesManagementCreateOrderStatusLabel.Content = "Open";
        _productionStagesManagementCreateOrderDateLabel.Content = DateTime.Now.ToString("dd.MM.yyyy");
        _productionStagesManagementCreateOrderToOrderButtonLabel.Content = "To order";
        fill_forStageComboBox();
        fill_productComboBox();
    }
    private void fill_forStageComboBox()
    {
        var stages = dbConnector.ProductionStages.ToList();
        _productionStagesManagementCreateOrderForStageComboBox.ItemsSource = stages;
        _productionStagesManagementCreateOrderForStageComboBox.DisplayMemberPath = "ProductionStageName";
    }
    private void fill_productComboBox()
    {
        var products = dbConnector.Products.ToList();
        _productionStagesManagementCreateOrderProductComboBox.ItemsSource = products;
        _productionStagesManagementCreateOrderProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void productionStagesManagementCreateOrderProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }
    private void productionStagesManagementCreateOrderCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        defaultState();
    }
    private void productionStagesManagementCreateOrderToOrderButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_productionStagesManagementCreateOrderForStageComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice production stage!").Show();
            return;
        }
        if (_productionStagesManagementCreateOrderProductComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice product!").Show();
            return;
        }
        if (!int.TryParse(_productionStagesManagementCreateOrderQuantityTextBox.Text.Trim(), out int quantity) || quantity <= 0)
        {
            new MessageWindow("Error", "Enter valid quantity!").Show();
            return;
        }

        int orderId = int.Parse(_productionStagesManagementCreateOrderDocumentIdLabel.Content.ToString()!);

        if (_productionStagesManagementCreateOrderToOrderButtonLabel.Content.ToString() == "Save")
            update_ProductionOrder(orderId);
        else
            add_newProductionOrder(orderId);

        this.Close();
    }
    private void add_newProductionOrder(int orderId)
    {
        int productionStageId = (_productionStagesManagementCreateOrderForStageComboBox.SelectedItem as production_stage_entity)?.ProductionStageId ?? throw new NullReferenceException();
        int productId = (_productionStagesManagementCreateOrderProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_productionStagesManagementCreateOrderQuantityTextBox.Text.Trim());

        dbConnector.ProductionOrders.Add(new production_order_entity
        {
            ProductionOrderId = orderId,
            IssueDate = DateTime.Now,
            Status = "Open",
            ProductId = productId,
            Quantity = quantity,
            ProductionStageId = productionStageId
        });

        dbConnector.SaveChanges();
    }
    private void update_ProductionOrder(int orderId)
    {
        int productionStageId = (_productionStagesManagementCreateOrderForStageComboBox.SelectedItem as production_stage_entity)?.ProductionStageId ?? throw new NullReferenceException();
        int productId = (_productionStagesManagementCreateOrderProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
        int quantity = int.Parse(_productionStagesManagementCreateOrderQuantityTextBox.Text.Trim());

        var existingOrder = dbConnector.ProductionOrders.FirstOrDefault(x => x.ProductionOrderId == orderId);
        if (existingOrder != null)
        {
            existingOrder.ProductionStageId = productionStageId;
            existingOrder.ProductId = productId;
            existingOrder.Quantity = quantity;
        }

        dbConnector.SaveChanges();
    }
}