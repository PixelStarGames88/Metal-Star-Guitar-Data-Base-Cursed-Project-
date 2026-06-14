using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars;

public partial class ProductRegisterWindow : Window
{
    private DataBaseConnector dbConnector;
    public ProductRegisterWindow(DataBaseConnector dateBaseConnector)
    {
        InitializeComponent();
        dbConnector = dateBaseConnector;

        _productionStagesManagementProductRegisterButtonLabel.MouseDown += productionStagesManagementProductRegisterButtonLabel_MouseDown;
        _productionStagesManagementProductCancelButtonLabel.MouseDown += productionStagesManagementProductCancelButtonLabel_MouseDown;

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
        _productionStagesManagementProductsProductsNameTextBox.Text = string.Empty;
        _productionStagesManagementProductsPriceTextBox.Text = string.Empty;
        _productionStagesManagementProductProductIdLabel.Content = ((dbConnector.Products.Max(p => (int?)p.ProductId) ?? 0) + 1).ToString();
        _productionStagesManagementProductRegisterButtonLabel.Content = "Register";
    }
    private void productionStagesManagementProductCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        defaultState();
    }
    private void productionStagesManagementProductRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_productionStagesManagementProductsProductsNameTextBox.Text))
        {
            new MessageWindow("Error", "Enter product name!").Show();
            return;
        }
        if (!decimal.TryParse(_productionStagesManagementProductsPriceTextBox.Text.Trim(), out decimal price))
        {
            new MessageWindow("Error", "Enter valid price!").Show();
            return;
        }

        int productId = int.Parse(_productionStagesManagementProductProductIdLabel.Content.ToString()!);

        if (_productionStagesManagementProductRegisterButtonLabel.Content.ToString() == "Save")
            update_Product(productId);
        else
            add_newProduct(productId);

        this.Close();
    }
    private void add_newProduct(int productId)
    {
        string productName = _productionStagesManagementProductsProductsNameTextBox.Text.Trim();
        decimal price = decimal.Parse(_productionStagesManagementProductsPriceTextBox.Text.Trim());

        dbConnector.Products.Add(new product_entity
        {
            ProductId = productId,
            ProductName = productName,
            Price = price
        });

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_ProductListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
    private void update_Product(int productId)
    {
        string productName = _productionStagesManagementProductsProductsNameTextBox.Text.Trim();
        decimal price = decimal.Parse(_productionStagesManagementProductsPriceTextBox.Text.Trim());

        var existingProduct = dbConnector.Products.FirstOrDefault(x => x.ProductId == productId);
        if (existingProduct != null)
        {
            existingProduct.ProductName = productName;
            existingProduct.Price = price;
        }

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_ProductListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
}