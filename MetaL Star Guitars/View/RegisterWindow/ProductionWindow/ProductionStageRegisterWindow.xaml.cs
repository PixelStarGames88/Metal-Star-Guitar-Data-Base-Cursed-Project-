using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow;

public partial class ProductionStageRegisterWindow : Window
{
    private DataBaseConnector dbConnector;
    public ProductionStageRegisterWindow(DataBaseConnector dateBaseConnector)
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
        _productionStagesManagementCreateStageNameTextBox.Text = string.Empty;
        _productionStagesManagementCreateStageResultProductComboBox.Text = string.Empty;
        _productionStagesManagementCreateStageIdLabel.Content = ((dbConnector.ProductionStages.Max(p => (int?)p.ProductionStageId) ?? 0) + 1).ToString();
        _productionStagesManagementCreateStageRegisterButtonLabel.Content = "Register";
        fill_productsListBox();
        fill_resultProductComboBox();
    }
    private void fill_resultProductComboBox()
    {
        var products = dbConnector.Products.ToList();
        _productionStagesManagementCreateStageResultProductComboBox.ItemsSource = products;
        _productionStagesManagementCreateStageResultProductComboBox.DisplayMemberPath = "ProductName";
    }
    private void fill_productsListBox()
    {
        var rawData = dbConnector.Products
            .Select(p => new
            {
                ProductDescribe = p.ProductName + " (ID: " + p.ProductId.ToString() + ")",
                ProductId = p.ProductId
            })
            .ToList();

        var products = rawData.Select(x => {
            dynamic item = new ExpandoObject();
            item.ProductDescribe = x.ProductDescribe;
            item.ProductId = x.ProductId;
            item.IsSelected = false;
            item.Quantity = 0;
            return (object)item;
        }).ToList();

        _productsListBox.ItemsSource = products;
    }
    private void productionStagesManagementCreateStageCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        defaultState();
    }
    private void productionStagesManagementCreateStageRegisterButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_productionStagesManagementCreateStageNameTextBox.Text))
        {
            new MessageWindow("Error", "Enter stage name!").Show();
            return;
        }
        if (_productionStagesManagementCreateStageResultProductComboBox.SelectedItem == null)
        {
            new MessageWindow("Error", "Choice result product!").Show();
            return;
        }

        int stageId = int.Parse(_productionStagesManagementCreateStageIdLabel.Content.ToString()!);

        if (_productionStagesManagementCreateStageRegisterButtonLabel.Content.ToString() == "Save")
            update_ProductionStage(stageId);
        else
            add_newProductionStage(stageId);

        add_newProductUsings(stageId);

        this.Close();
    }
    private void add_newProductionStage(int stageId)
    {
        string stageName = _productionStagesManagementCreateStageNameTextBox.Text.Trim();
        int resultProductId = (_productionStagesManagementCreateStageResultProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();

        dbConnector.ProductionStages.Add(new production_stage_entity
        {
            ProductionStageId = stageId,
            ProductionStageName = stageName,
            ResultProductId = resultProductId
        });

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_StageListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
    private void update_ProductionStage(int stageId)
    {
        string stageName = _productionStagesManagementCreateStageNameTextBox.Text.Trim();
        int resultProductId = (_productionStagesManagementCreateStageResultProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();

        var existingStage = dbConnector.ProductionStages.FirstOrDefault(x => x.ProductionStageId == stageId);
        if (existingStage != null)
        {
            existingStage.ProductionStageName = stageName;
            existingStage.ResultProductId = resultProductId;
        }

        var oldUsings = dbConnector.ProductUsings.Where(pu => pu.ProductionStageId == stageId).ToList();
        dbConnector.ProductUsings.RemoveRange(oldUsings);

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        mainWindow?.fill_StageListBox();
        new MessageWindow("Message", "Changes are successfull!").Show();
        dbConnector.SaveChanges();
        this.Close();
    }
    private void add_newProductUsings(int stageId)
    {
        var selectedProducts = _productsListBox.Items.Cast<dynamic>()
            .Where(x => x.IsSelected)
            .Select(x => new product_using_entity
            {
                ProductionStageId = stageId,
                ProductId = (int)x.ProductId,
                Quantity = int.Parse(x.Quantity.ToString())
            })
            .ToList();

        dbConnector.ProductUsings.AddRange(selectedProducts);
        dbConnector.SaveChanges();
    }

    private void _productsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}