using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MetaL_Star_Guitars.DataBase.Connection;

namespace MetaL_Star_Guitars
{
    public partial class MainWindow : Window
    {
        DataBaseConnector dbConnector;
        public MainWindow()
        {
            InitializeComponent();
            dbConnector = new DataBaseConnector();
        }
        private void _authorizationEnterButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var selectedRole = (_authorizationPostComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _authorizationGrid.Visibility = Visibility.Collapsed;

            if (selectedRole == "Warehouse manager")
            {
                _warehouseManagementModuleGrid.Visibility = Visibility.Visible;
                _productionStagesManagementModuleGrid.Visibility = Visibility.Collapsed;

                _warehouseTransfersSubGrid.Visibility = Visibility.Visible;
                _warehouseWriteOffToProductionSubGrid.Visibility = Visibility.Collapsed;
            }
            else if (selectedRole == "Production stages manager")
            {
                _warehouseManagementModuleGrid.Visibility = Visibility.Collapsed;
                _productionStagesManagementModuleGrid.Visibility = Visibility.Visible;

                _productionCreateOrderSubGrid.Visibility = Visibility.Visible;
                _productionReleaseProductsSubGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void _warehouseNavigationTransfersButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _warehouseTransfersSubGrid.Visibility = Visibility.Visible;
            _warehouseWriteOffToProductionSubGrid.Visibility = Visibility.Collapsed;
        }

        private void _warehouseNavigationWriteOffToProductionButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _warehouseTransfersSubGrid.Visibility = Visibility.Collapsed;
            _warehouseWriteOffToProductionSubGrid.Visibility = Visibility.Visible;
        }
        private void _productionNavigationCreateOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _productionCreateOrderSubGrid.Visibility = Visibility.Visible;
            _productionReleaseProductsSubGrid.Visibility = Visibility.Collapsed;
        }

        private void _productionNavigationReleaseProductsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _productionCreateOrderSubGrid.Visibility = Visibility.Collapsed;
            _productionReleaseProductsSubGrid.Visibility = Visibility.Visible;
        }
        private void _globalExitToMenuButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _authorizationGrid.Visibility = Visibility.Visible;
            _warehouseManagementModuleGrid.Visibility = Visibility.Collapsed;
            _productionStagesManagementModuleGrid.Visibility = Visibility.Collapsed;
        }

        private void _globalButton_MouseEnterYellow(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.Yellow;
        }

        private void _globalButton_MouseLeaveWhite(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.White;
        }

        private void _warehouseTransferSubmitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dbConnector.CreateTransferOrder(new DataBase.Entities.transfer_order_entity         
                {
                    ShipmentDate = DateTime.UtcNow,//DateTime.Parse(_warehouseTransferShipmentDateValueLabel.Content.ToString()),
                    EstimatedDeliveryDate = DateTime.UtcNow,//DateTime.Parse(_warehouseTransferReceiptDateValueLabel.Content.ToString()),
                    Status = "Open",//_warehouseTransferStatusValueLabel.Content.ToString(),
                    SenderWarehouseId = 1,//int.Parse(_warehouseTransferSenderWarehouseComboBox.SelectedValue.ToString()),
                    RecipientWarehouseId = 2,//int.Parse(_warehouseTransferRecipientWarehouseComboBox.SelectedValue.ToString()),
                    RouteId = 1//int.Parse(_warehouseTransferRouteComboBox.SelectedValue.ToString())
                });
        }

        private void _warehouseWriteOffSubmitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}