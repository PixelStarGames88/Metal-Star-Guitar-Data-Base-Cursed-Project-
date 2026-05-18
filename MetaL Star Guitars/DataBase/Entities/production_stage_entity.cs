using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;


[Table("production_stage", Schema = "public")]
public class production_stage_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("production_stage_id")]
    public int ProductionStageId { get; set; }

    [Column("production_stage_name")]
    public string ProductionStageName { get; set; } = null!;

    [Column("result_product_id")]
    public int ResultProductId { get; set; }
}
