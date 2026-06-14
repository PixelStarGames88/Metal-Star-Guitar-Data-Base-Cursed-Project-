using MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{

    private void productionStagesManagementStagesButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _productionStagesManagementProductsGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementReleaseProductGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementProductionOrdersGrid.Visibility = Visibility.Collapsed;
        _productionStagesManagementStagesGrid.Visibility = Visibility.Visible;
        fill_StageListBox();
    }
    public void fill_StageListBox()
    {
        var stages = dbConnector.ProductionStages
            .Join
            (
                dbConnector.Products, ps => ps.ResultProductId, p => p.ProductId,
                (ps, p) => new
                {
                    stageId = ps.ProductionStageId,
                    stageName = ps.ProductionStageName,
                    productName = p.ProductName
                }
            ).ToList();

        _stageListBox.ItemsSource = stages;
    }
    private void _createStageButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new ProductionStageRegisterWindow(dbConnector).Show();
    }
    private void editStageEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int stageId = item.stageId;
        var entity = dbConnector.ProductionStages.FirstOrDefault(x => x.ProductionStageId == stageId);

        if (entity != null)
        {
            var w = new ProductionStageRegisterWindow(dbConnector);

            w._productionStagesManagementCreateStageIdLabel.Content = entity.ProductionStageId;
            w._productionStagesManagementCreateStageNameTextBox.Text = entity.ProductionStageName;
            w._productionStagesManagementCreateStageResultProductComboBox.SelectedItem =
                dbConnector.Products.FirstOrDefault(p => p.ProductId == entity.ResultProductId);

            var usedProducts = dbConnector.ProductUsings
                .Where(pu => pu.ProductionStageId == stageId)
                .Select(pu => new { pu.ProductId, pu.Quantity })
                .ToList();

            var productsList = w._productsListBox.ItemsSource.Cast<dynamic>()
                .Select(x =>
                {
                    int productId = (int)x.ProductId;
                    var usedProduct = usedProducts.FirstOrDefault(up => up.ProductId == productId);

                    x.IsSelected = usedProduct != null;
                    x.Quantity = usedProduct?.Quantity ?? 0;

                    return (object)x;
                })
                .ToList();

            w._productsListBox.ItemsSource = productsList;

            w._productionStagesManagementCreateStageRegisterButtonLabel.Content = "Save";
            w.Show();
        }
    }
    private void deleteStageEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int stageId = item.stageId;

        var oldUsings = dbConnector.ProductUsings.Where(pu => pu.ProductionStageId == stageId).ToList();
        dbConnector.ProductUsings.RemoveRange(oldUsings);

        var entity = dbConnector.ProductionStages.FirstOrDefault(x => x.ProductionStageId == stageId);
        if (entity != null)
        {
            dbConnector.ProductionStages.Remove(entity);
        }

        dbConnector.SaveChanges();
        fill_StageListBox();
    }
}