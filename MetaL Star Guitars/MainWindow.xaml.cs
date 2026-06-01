using MetaL_Star_Guitars.DataBase.Connection;
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
            productionStagesManagementReleaseProductsButton_MouseDown(sender, e);
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

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}