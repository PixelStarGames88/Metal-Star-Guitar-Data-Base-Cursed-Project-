using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transit_warehouse_route", Schema = "public")]
public class transit_warehouse_route_entity
{
    [Column("transit_warehouse_id")]
    public int TransitWarehouseId { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }
}