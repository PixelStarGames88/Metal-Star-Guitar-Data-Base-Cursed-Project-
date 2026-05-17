using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transfer_order", Schema = "public")]
public class TransferOrderEntity
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
}