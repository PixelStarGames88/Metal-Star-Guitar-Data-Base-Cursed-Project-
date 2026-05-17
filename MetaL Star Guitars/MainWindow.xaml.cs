using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetaL_Star_Guitars
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Логика авторизации
        private void _authorizationEnterButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var selectedRole = (_authorizationPostComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _authorizationGrid.Visibility = Visibility.Collapsed;

            if (selectedRole == "Warehouse manager")
            {
                _warehouseManagementModuleGrid.Visibility = Visibility.Visible;
                _productionStagesManagementModuleGrid.Visibility = Visibility.Collapsed;

                // Дефолтные вкладки для Склада
                _warehouseTransfersSubGrid.Visibility = Visibility.Visible;
                _warehouseWriteOffToProductionSubGrid.Visibility = Visibility.Collapsed;
            }
            else if (selectedRole == "Production stages manager")
            {
                _warehouseManagementModuleGrid.Visibility = Visibility.Collapsed;
                _productionStagesManagementModuleGrid.Visibility = Visibility.Visible;

                // Дефолтные вкладки для Производства
                _productionCreateOrderSubGrid.Visibility = Visibility.Visible;
                _productionReleaseProductsSubGrid.Visibility = Visibility.Collapsed;
            }
        }

        // --- НАВИГАЦИЯ: МОДУЛЬ СКЛАДОВ ---
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

        // --- НАВИГАЦИЯ: МОДУЛЬ ПРОИЗВОДСТВА ---
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

        // --- ГЛОБАЛЬНЫЕ МЕТОДЫ И ИНТЕРФЕЙСНЫЕ СУЩНОСТИ ---
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
    }
}