using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("final_route", Schema = "public")]
public class final_route_entity
{
    [Key]
    [Column("final_route_id")]
    public string? FinalRouteId { get; set; }

    [Column("scheduled_time")]
    public TimeSpan ScheduledTime { get; set; }
}