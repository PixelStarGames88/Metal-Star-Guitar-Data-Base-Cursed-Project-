using MetaL_Star_Guitars.DataBase.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementTransfersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Visible;
        warehouseManagementTransfersCancelButton_MouseDown(sender, e);
        fillRecepientWarehouseComboBox();
        fillSenderWarehouseComboBox();
    }
    private void fillRecepientWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementTransfersRecipientWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementTransfersRecipientWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void fillSenderWarehouseComboBox()
    {
        var warehouses = dbConnector.Warehouses.ToList();
        _warehouseManagementTransfersSenderWarehouseComboBox.ItemsSource = warehouses;
        _warehouseManagementTransfersSenderWarehouseComboBox.DisplayMemberPath = "WarehouseName";
    }
    private void warehouseManagementTransfersCancelButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransfersSenderWarehouseComboBox.Text = string.Empty;
        _warehouseManagementTransfersRecipientWarehouseComboBox.Text = string.Empty;
        _warehouseManagementTransfersRouteComboBox.Text = string.Empty;
        int maxId = dbConnector.TransferOrders.Max(p => (int?)p.TransferOrderId) ?? 0;
        _warehouseManagementTransfersOrderIdLabel.Content = (maxId + 1).ToString();
        _warehouseManagementTransfersShipmentDateLabel.Content = DateTime.Now.ToString("dd.MM.yyyy");
        _warehouseManagementTransfersReceiptDateLabel.Content = "Choice route.";
    }
    private void warehouseManagementTransfersSenderWarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string warehousename = null!;

        if (_warehouseManagementTransfersSenderWarehouseComboBox.SelectedItem is warehouse_entity selectedWarehouse)
            warehousename = selectedWarehouse.WarehouseName;

        var products = dbConnector.Products.
        Join(dbConnector.Stocks,
            p => p.ProductId, s => s.ProductId,
            (p, s) => new { 
                Product = p, 
                Stock = s }
            ).
        Join(dbConnector.Warehouses,
            combined => combined.Stock.WarehouseId,
            w => w.WarehouseId,
            (combined, w) =>
            new { 
                combined.Product, 
                Warehouse = w,
                ProductDescribe = (combined.Product.ProductName + " (" + combined.Stock.Quantity.ToString() + " pcs.)")
            }).
        Where(x => x.Warehouse.WarehouseName == warehousename)
    .Select(x => new { x.ProductDescribe, x.Product.ProductId })
    .Distinct()
    .ToList();

        _contentListBox.ItemsSource = products;
    }
}