using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("product_using", Schema = "public")]
public class product_using_entity
{
    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("production_stage_id")]
    public int ProductionStageId { get; set; }
}
