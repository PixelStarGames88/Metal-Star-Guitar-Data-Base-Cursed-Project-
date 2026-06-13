using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetaL_Star_Guitars.View.RegisterWindow.ProductionWindow
{
    /// <summary>
    /// Логика взаимодействия для ProductionStageRegisterWindow.xaml
    /// </summary>
    public partial class ProductionStageRegisterWindow : Window
    {
        public ProductionStageRegisterWindow()
        {
            InitializeComponent();
        }
        private void someButton_MouseEnterYellow(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.Yellow;
        }
        private void someButton_MouseLeaveWhite(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.White;
        }
    }
}
