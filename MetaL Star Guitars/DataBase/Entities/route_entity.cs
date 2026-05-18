using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("route", Schema = "public")]
public class route_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("travel_time")]
    public TimeSpan TravelTime { get; set; }

    [Column("start_warehouse_id")]
    public int StartWarehouseId { get; set; }

    [Column("finish_warehouse_id")]
    public int FinishWarehouseId { get; set; }
}
