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
    private void warehouseManagementRoutesButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransfersGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementRoutesGrid.Visibility = Visibility.Visible;

        fill_RouteListBox();
    }
    private void fill_RouteListBox()
    {
        var documents = dbConnector.Routes
            .Join
            (
                dbConnector.Warehouses, r => r.StartWarehouseId, w => w.WarehouseId,
                (r, w) => new
                {
                    route = r,
                    startWarehouse = w
                }
            )
            .Join
            (
                dbConnector.Warehouses, r => r.route.FinishWarehouseId, w => w.WarehouseId,
                (r, w) => new
                {
                    routeId = r.route.RouteId,
                    routeTime = r.route.TravelTime,
                    startWarehouse = r.startWarehouse.WarehouseName,
                    finishWarehouse = w.WarehouseName
                }
            ).ToList();

        _routeListBox.ItemsSource = documents;
    }
    private void deleteRouteEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int routeId = item.routeId;
        var entity = dbConnector.Routes.FirstOrDefault(x => x.RouteId == routeId);
        if (entity != null) { dbConnector.Routes.Remove(entity); dbConnector.SaveChanges(); fill_RouteListBox(); }
    }
    private void editRouteEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int routeId = item.routeId;
        var entity = dbConnector.Routes.FirstOrDefault(x => x.RouteId == routeId);

        if (entity != null)
        {
            var w = new RouteRegisterWindow(dbConnector);

            w._warehouseManagementRouteRouteIdLabel.Content = entity.RouteId;
            w._warehouseManagementRouteTimeTextBox.Text = entity.TravelTime.ToString();

            var startWarehouse = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == entity.StartWarehouseId);
            var finishWarehouse = dbConnector.Warehouses.FirstOrDefault(x => x.WarehouseId == entity.FinishWarehouseId);

            w._warehouseManagementRouteStartWarehouseComboBox.SelectedItem = startWarehouse;
            w._warehouseManagementRouteFinishWarehouseComboBox.SelectedItem = finishWarehouse;

            var transitIds = dbConnector.TransitWarehouseRoutes
                .Where(twr => twr.RouteId == routeId)
                .Select(twr => twr.TransitWarehouseId)
                .ToList();

            var transitList = w._transitWarehousesListBox.ItemsSource.Cast<dynamic>()
                .Select(x => {
                    x.IsSelected = transitIds.Contains((int)x.IdWarehouse);
                    return (object)x;
                })
                .ToList();

            w._transitWarehousesListBox.ItemsSource = transitList;

            w._warehouseManagementRouteRegisterButtonLabel.Content = "Save";
            w.Show();
        }
    }
    private void _createRouteButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        RouteRegisterWindow routeRegisterWindow = new RouteRegisterWindow(dbConnector);
        routeRegisterWindow.Show();
        if(routeRegisterWindow.DialogResult != null && routeRegisterWindow.DialogResult.Value)
        {
            fill_RouteListBox();
        }
    }
}
