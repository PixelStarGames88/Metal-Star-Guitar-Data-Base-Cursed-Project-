using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("product", Schema = "public")]
public class product_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    public string ProductName { get; set; } = null!;

    [Column("price")]
    public decimal Price { get; set; }
    //public List<product_using_entity> ProductUsings { get; set; } = new();
    //public List<production_order_entity> ProductionOrders { get; set; } = new();
    //public List<stock_adjustment_document_entity> StockAdjustmentDocumentEntities { get; set; } = new();
    //public List<stock_entity> StockEntities { get; set; } = new();
    //public List<transfer_order_content_entity> TransferOrderContentEntities { get; set;} = new();
    //public List<production_stage_entity> ProductionStageEntities { get; set; } = new();
}
