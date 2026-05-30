using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("stock_adjustment_document", Schema = "public")]
public class stock_adjustment_document_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("stock_adjustment_document_id")]
    public int StockAdjustmentDocumentId { get; set; }

    [Column("issue_date")]
    public DateTime IssueDate { get; set; }

    [Column("document_type")]
    public string DocumentType { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("production_order_id")]
    public int ProductionOrderId { get; set; }
    
    //public warehouse_entity? Warehouse { get; set; }
    //public product_entity? Product { get; set; }
    //public production_order_entity? ProductionOrder { get; set; }
}
