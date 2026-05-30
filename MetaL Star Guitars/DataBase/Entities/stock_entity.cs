using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("stock", Schema = "public")]
public class stock_entity
{
    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }
    //public warehouse_entity? Warehouse { get; set; }
    //public product_entity? Product { get; set; }
}
