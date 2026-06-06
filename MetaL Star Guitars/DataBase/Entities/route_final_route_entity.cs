using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("route_final_route", Schema = "public")]
public class route_final_route_entity
{
    [Column("final_route_id")]
    public string? FinalRouteId { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("sequence_route")]
    public int SequenceRoute { get; set; }
    public route_entity Route { get; set; }

    public final_route_entity FinalRoute { get; set; }
}