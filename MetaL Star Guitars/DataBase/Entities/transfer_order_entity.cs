using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transfer_order", Schema = "public")]
public class transfer_order_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("transfer_order_id")]
    public int TransferOrderId { get; set; }

    [Column("shipment_date")]
    public DateTime ShipmentDate { get; set; }

    [Column("estimated_delivery_date")]
    public DateTime EstimatedDeliveryDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    [Column("recipient_warehouse_id")]
    public int RecipientWarehouseId { get; set; }

    [Column("sender_warehouse_id")]
    public int SenderWarehouseId { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }
    //public route_entity? Route { get; set; }
    //public warehouse_entity? RecipientWarehouse { get; set; }
    //public warehouse_entity? SenderWarehouse { get; set; }
    //public List<transfer_transaction_entity> TransferTransactions { get; set; } = new();
    //public List<transfer_order_content_entity> TransferOrderContent { get; set; } = new();
}
