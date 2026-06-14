using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void productionStagesManagementProductsButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementStagesGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductsGrid.Visibility = Visibility.Visible;
        fill_ProductListBox();
    }
    private void _createProductButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ProductRegisterWindow(dbConnector).Show();
    }
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
    public void fill_ProductListBox()
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
}