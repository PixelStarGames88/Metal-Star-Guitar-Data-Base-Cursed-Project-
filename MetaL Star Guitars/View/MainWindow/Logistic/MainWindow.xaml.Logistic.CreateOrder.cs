using MetaL_Star_Guitars.DataBase.Entities;
using MetaL_Star_Guitars.View.RegisterWindow;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaL_Star_Guitars;

public partial class MainWindow : Window
{
    private void warehouseManagementTransfersButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _warehouseManagementTransactionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementRoutesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWriteOffToProductionGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementWarehousesGrid.Visibility = Visibility.Collapsed;
        _warehouseManagementTransitWarehousesGrid.Visibility = Visibility.Collapsed;

        _warehouseManagementTransfersGrid.Visibility = Visibility.Visible;

        fill_TransferOrderListBox();
    }
    public void fill_TransferOrderListBox()
    {
        var documents = dbConnector.TransferOrders
            .Join
            (
                dbConnector.Warehouses, to => to.SenderWarehouseId, w => w.WarehouseId,
                (to, w) => new
                {
                    order = to,
                    senderWarehouse = w
                }
            )
            .Join
            (
                dbConnector.Warehouses, to => to.order.RecipientWarehouseId, w => w.WarehouseId,
                (to, w) => new
                {
                    orderId = to.order.TransferOrderId,
                    orderStartData = to.order.ShipmentDate,
                    orderStatus = to.order.Status,
                    orderFinishData = to.order.EstimatedDeliveryDate,
                    senderWarehouse = to.senderWarehouse.WarehouseName,
                    recipientWarehouse = w.WarehouseName
                }
            ).ToList();

        _transferOrdersListBox.ItemsSource = documents;
    }

    private void deleteTransferOrderEntity(object sender, MouseButtonEventArgs e)
    {
        dynamic item = (sender as TextBlock)?.DataContext ?? throw new NullReferenceException();
        int orderId = item.orderId;

        var transactions = dbConnector.TransferTransactions
            .Where(x => x.TransferOrderId == orderId)
            .ToList();

        if (transactions.Any())
        {
            dbConnector.TransferTransactions.RemoveRange(transactions);
            dbConnector.SaveChanges();
        }

        var contents = dbConnector.TransferOrderContents
            .Where(x => x.TransferOrderId == orderId)
            .ToList();

        if (contents.Any())
        {
            dbConnector.TransferOrderContents.RemoveRange(contents);
            dbConnector.SaveChanges();
        }

        var entity = dbConnector.TransferOrders.FirstOrDefault(x => x.TransferOrderId == orderId);
        if (entity != null)
        {
            dbConnector.TransferOrders.Remove(entity);
            dbConnector.SaveChanges();
        }

        fill_TransferOrderListBox();
    }
    private void _createTransferOrderButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new TransferOrderRegisterWindow(dbConnector).Show();
    }
}