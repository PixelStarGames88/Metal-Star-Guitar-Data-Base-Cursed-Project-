using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transit_warehouse", Schema = "public")]
public class transit_warehouse_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("transit_warehouse_id")]
    public int TransitWarehouseId { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("type_transit")]
    public string TypeTransit { get; set; } = null!;
}
