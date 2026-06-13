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
}
