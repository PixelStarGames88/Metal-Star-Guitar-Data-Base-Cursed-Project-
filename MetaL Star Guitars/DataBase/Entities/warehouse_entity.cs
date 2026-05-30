using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("warehouse", Schema = "public")]
public class warehouse_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("warehouse_name")]
    public string WarehouseName { get; set; } = null!;

    [Column("capacity")]
    public int Capacity { get; set; }
    //public List<stock_adjustment_document_entity> StockAdjustmentDocuments { get; set; } = new();
    //public List<stock_entity> StockEntities { get; set; } = new();
    //public List<transfer_order_entity> SenderTransferOrderEntities { get; set; } = new();
    //public List<transfer_order_entity> RecipientTransferOrderEntities { get; set; } = new();
    //public List<production_order_entity> ProductionOrderEntities { get; set; } = new();
    //public List<route_entity> StartRouteEntities { get; set; } = new();
    //public List<route_entity> EndRouteEntities { get; set; } = new();
}
