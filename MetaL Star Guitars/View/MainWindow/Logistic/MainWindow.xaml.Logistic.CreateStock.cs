using MetaL_Star_Guitars.DataBase.Entities;
using MetaL_Star_Guitars.View.RegisterWindow.LogisticWindow;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    public void fill_StockListBox()
    {
        var stock = dbConnector.Stocks
            .Join
            (
                dbConnector.Warehouses, s => s.WarehouseId, w => w.WarehouseId,
                (s, w) => new
                {
                    stock = s,
                    Warehouse = w
                }
            )
            .Join
            (
                dbConnector.Products, s => s.stock.ProductId, p => p.ProductId,
                (s, p) => new
                {
                    stock = s.stock.Quantity,
                    warehouse = s.Warehouse.WarehouseName,
                    product = p.ProductName
                }
            ).ToList();

        _stockListBox.ItemsSource = stock;
    }
    private void deleteStockEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        string warehouseName = item.warehouse;
        string productName = item.product;

        int warehouseId = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseName == warehouseName)?.WarehouseId ?? throw new NullReferenceException();
        int productId = dbConnector.Products.FirstOrDefault(p => p.ProductName == productName)?.ProductId ?? throw new NullReferenceException();

        var entity = dbConnector.Stocks.FirstOrDefault(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (entity != null) { dbConnector.Stocks.Remove(entity); dbConnector.SaveChanges(); fill_StockListBox(); }
    }

    private void editStockEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        string warehouseName = item.warehouse;
        string productName = item.product;

        int warehouseId = dbConnector.Warehouses.FirstOrDefault(w => w.WarehouseName == warehouseName)?.WarehouseId ?? throw new NullReferenceException();
        int productId = dbConnector.Products.FirstOrDefault(p => p.ProductName == productName)?.ProductId ?? throw new NullReferenceException();

        var entity = dbConnector.Stocks.FirstOrDefault(x => x.WarehouseId == warehouseId && x.ProductId == productId);

        if (entity != null)
        {
            var w = new StockRegisterWindow(dbConnector);
            w._warehouseManagementWarehouseWarehouseComboBox.SelectedItem = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == entity.WarehouseId);
            w._warehouseManagementWarehouseProductComboBox.SelectedItem = dbConnector.Products.FirstOrDefault(x => x.ProductId == entity.ProductId);
            w._warehouseManagementWarehousesQuantityTextBox.Text = entity.Quantity.ToString();
            w.Show();
        }
    }
    private void _createStockButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        StockRegisterWindow stockRegisterWindow = new StockRegisterWindow(dbConnector);
        stockRegisterWindow.Show();
    }
}