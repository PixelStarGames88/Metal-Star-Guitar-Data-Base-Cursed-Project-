using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("production_order", Schema = "public")]
public class ProductionOrderEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("production_order_id")]
    public int ProductionOrderId { get; set; }

    [Column("issue_date")]
    public DateTime IssueDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("production_stage_id")]
    public int ProductionStageId { get; set; }
}
