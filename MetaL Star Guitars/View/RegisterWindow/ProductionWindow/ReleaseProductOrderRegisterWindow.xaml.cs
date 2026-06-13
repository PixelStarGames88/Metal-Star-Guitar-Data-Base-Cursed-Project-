using MetaL_Star_Guitars.DataBase.Connection;
using MetaL_Star_Guitars.DataBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Логика взаимодействия для ReleaseProductOrderRegisterWindow.xaml
    /// </summary>
    public partial class ReleaseProductOrderRegisterWindow : Window
    {
        private DataBaseConnector dbConnector;
        public ReleaseProductOrderRegisterWindow(DataBaseConnector dateBaseConnector)
        {
            InitializeComponent();
            dbConnector = dateBaseConnector;

        }
        private void someButton_MouseEnterYellow(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.Yellow;
        }
        private void someButton_MouseLeaveWhite(object sender, MouseEventArgs e)
        {
            if (sender is Label label) label.Foreground = Brushes.White;
        }
        private void fillFromWarehouseComboBoxProductionStagesManagement()
        {
            var warehouses = dbConnector.Warehouses.ToList();
            _productionStagesManagementReleaseProductsToWarehouseComboBox.ItemsSource = warehouses;
            _productionStagesManagementReleaseProductsToWarehouseComboBox.DisplayMemberPath = "WarehouseName";
        }
        private void fillForOrderComboBoxProductionStagesManagement()
        {
            var orders = dbConnector.ProductionOrders.
            Join(dbConnector.Products,
            o => o.ProductId, p => p.ProductId,
            (o, p) => new
            {
                Product = p,
                ProductionOrder = o
            }).Join(dbConnector.ProductionStages,
            o => o.ProductionOrder.ProductionStageId, ps => ps.ProductionStageId,
            (o, ps) => new
            {
                ProductionOrder = o.ProductionOrder,
                OrderDescription = (o.Product.ProductName + " | " + ps.ProductionStageName + " | " + o.ProductionOrder.Quantity)
            }).ToList();
            _productionStagesManagementReleaseProductsToOrderComboBox.ItemsSource = orders;
            _productionStagesManagementReleaseProductsToOrderComboBox.DisplayMemberPath = "OrderDescription";
        }
        private void productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _productionStagesManagementReleaseProductsToOrderComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsToWarehouseComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsProductComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsQuantityTextBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsTypeComboBox.Text = string.Empty;
            _productionStagesManagementReleaseProductsDateLabel.Content = DateTime.Now.ToString("HH\\:mm dd.MM.yyyy");
            int maxId = dbConnector.StockAdjustmentDocuments.Max(p => (int?)p.ProductId) ?? 0;
            _productionStagesManagementReleaseProductsDocumentIdLabel.Content = (maxId + 1).ToString();
            _productionStagesManagementReleaseProductsToReleaseButtonLabel.Content = "To release";
            fill_releaseProductsDocumentsListBox();
        }
        private void productionStagesManagementReleaseProductsToWarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string warehousename = null!;

            if (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem is warehouse_entity selectedWarehouse)
                warehousename = selectedWarehouse.WarehouseName;

            var products = dbConnector.Products.
            Join(dbConnector.Stocks,
                p => p.ProductId, s => s.ProductId,
                (p, s) => new { Product = p, Stock = s }).
            Join(dbConnector.Warehouses,
                combined => combined.Stock.WarehouseId,
                w => w.WarehouseId,
                (combined, w) =>
                new { combined.Product, Warehouse = w }).
            Where(x => x.Warehouse.WarehouseName == warehousename)
            .Select(x => x.Product).Distinct().ToList();

            _productionStagesManagementReleaseProductsProductComboBox.ItemsSource = products;
            _productionStagesManagementReleaseProductsProductComboBox.DisplayMemberPath = "ProductName";
        }
        private void productionStagesManagementReleaseProductsProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string productname = null!;

            if (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem is product_entity selectedProduct)
                productname = selectedProduct.ProductName;
        }
        private void productionStagesManagementReleaseProductsToReleaseButtonLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int productid = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductId ?? throw new NullReferenceException();
            int warehouseid = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseId ?? throw new NullReferenceException();
            dynamic? selectedItem = _productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem;
            int orderid = selectedItem.ProductionOrder.ProductionOrderId;
            int quantity;
            if (!int.TryParse(_productionStagesManagementReleaseProductsQuantityTextBox.Text, out quantity))
            {
                new MessageWindow("Error", "Enter integer number!").Show();
                return;
            }
            string? productname = (_productionStagesManagementReleaseProductsProductComboBox.SelectedItem as product_entity)?.ProductName ?? throw new NullReferenceException();
            string? warehousename = (_productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem as warehouse_entity)?.WarehouseName ?? throw new NullReferenceException();

            dbConnector.StockAdjustmentDocuments.Add(new stock_adjustment_document_entity
            {
                IssueDate = DateTime.UtcNow,
                DocumentType = _productionStagesManagementReleaseProductsTypeComboBox.Text,
                Quantity = quantity,
                WarehouseId = warehouseid,
                ProductionOrderId = orderid,
                ProductId = productid
            });
            dbConnector.SaveChanges();
            productionStagesManagementReleaseProductsCancelButtonLabel_MouseDown(sender, e);
            new MessageWindow("Message", "Your document was made successfully!").Show();
        }
        private void EditDocumentRelease_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is stock_adjustment_document_entity document)
            {
                _productionStagesManagementReleaseProductsDocumentIdLabel.Content = document.StockAdjustmentDocumentId.ToString();

                _productionStagesManagementReleaseProductsDateLabel.Content = document.IssueDate.ToString("HH:mm dd.MM.yyyy");

                _productionStagesManagementReleaseProductsQuantityTextBox.Text = document.Quantity.ToString();

                var warehouse = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseId == document.WarehouseId);

                if (warehouse != null)
                {
                    _productionStagesManagementReleaseProductsToWarehouseComboBox.SelectedItem = warehouse;
                }

                var type = document.DocumentType.ToString();

                if (type != null)
                {
                    if (type == "Finished product output")
                        _productionStagesManagementReleaseProductsTypeComboBox.SelectedIndex = 0;
                    else
                        _productionStagesManagementReleaseProductsTypeComboBox.SelectedIndex = 1;
                }

                var product = dbConnector.Products.FirstOrDefault(p => p.ProductId == document.ProductId);

                if (product != null)
                {
                    _productionStagesManagementReleaseProductsProductComboBox.SelectedItem = product;
                }

                var orderItem = _productionStagesManagementReleaseProductsToOrderComboBox
                .Items.Cast<dynamic>().FirstOrDefault(x => x.ProductionOrder.ProductionOrderId == document.ProductionOrderId);

                if (orderItem != null)
                {
                    _productionStagesManagementReleaseProductsToOrderComboBox.SelectedItem = orderItem;
                }

                _productionStagesManagementReleaseProductsToReleaseButtonLabel.Content = "Save";
            }
        }
        private void DeleteDocumentRelease_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock &&
                textBlock.DataContext is stock_adjustment_document_entity document)
            {
                var documentToDelete = dbConnector.StockAdjustmentDocuments
                    .FirstOrDefault(x => x.StockAdjustmentDocumentId == document.StockAdjustmentDocumentId);

                if (documentToDelete != null)
                {
                    dbConnector.StockAdjustmentDocuments.Remove(documentToDelete);
                    dbConnector.SaveChanges();

                    fill_releaseProductsDocumentsListBox();
                }
            }
        }
        private void fill_releaseProductsDocumentsListBox()
        {
            if (dbConnector.StockAdjustmentDocuments.Count() == 0)
                return;

            var Documents = dbConnector.StockAdjustmentDocuments.ToList();
        }
    }
}
