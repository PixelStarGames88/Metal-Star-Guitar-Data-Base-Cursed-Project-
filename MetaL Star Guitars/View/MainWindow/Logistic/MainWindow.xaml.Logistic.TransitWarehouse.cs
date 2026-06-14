using MetaL_Star_Guitars.View.RegisterWindow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementTransitWarehousesButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Visible;
        
        fill_TransitWarehouseListBox();
    }
    private void editTransitWarehouseEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int transitWarehouseId = item.TransitWarehouseId;
        var entity = dbConnector.TransitWarehouses.FirstOrDefault(x => x.TransitWarehouseId == transitWarehouseId);

        if (entity != null)
        {
            var w = new TransitWareHouseRegisterWindow(dbConnector);

            w._warehouseManagementTransitWarehouseIdLabel.Content = entity.TransitWarehouseId;
            w._warehouseManagementTransitWarehouseTypeTextBox.Text = entity.TypeTransit;
            w._warehouseManagementTransitWarehouseCapacityTextBox.Text = entity.Capacity.ToString();

            var routeIds = dbConnector.TransitWarehouseRoutes
                .Where(twr => twr.TransitWarehouseId == transitWarehouseId)
                .Select(twr => twr.RouteId)
                .ToList();

            var routesList = w._routesListBox.ItemsSource.Cast<dynamic>()
                .Select(x => {
                    x.IsSelected = routeIds.Contains((int)x.IdRoute);
                    return (object)x;
                })
                .ToList();

            w._routesListBox.ItemsSource = routesList;

            w._warehouseManagementTransitWarehouseRegisterButtonLabel.Content = "Save";
            w.Show();
        }
    }

    private void deleteTransitWarehouseEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int transitWarehouseId = item.TransitWarehouseId;

        var oldRouteLinks = dbConnector.TransitWarehouseRoutes.Where(x => x.TransitWarehouseId == transitWarehouseId).ToList();
        dbConnector.TransitWarehouseRoutes.RemoveRange(oldRouteLinks);

        var entity = dbConnector.TransitWarehouses.FirstOrDefault(x => x.TransitWarehouseId == transitWarehouseId);
        if (entity != null)
        {
            dbConnector.TransitWarehouses.Remove(entity);
        }

        dbConnector.SaveChanges();
        fill_TransitWarehouseListBox();
    }
    public void fill_TransitWarehouseListBox()
    {
        var documents = dbConnector.TransitWarehouses.ToList();

        _transitListBox.ItemsSource = documents;
    }
    private void _createTransitButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new TransitWareHouseRegisterWindow(dbConnector).Show();
    }
}
