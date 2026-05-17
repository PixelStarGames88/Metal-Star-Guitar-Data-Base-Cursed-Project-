using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transfer_order_content", Schema = "public")]
public class TransferOrderContentEntity
{
    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("transfer_order_id")]
    public int TransferOrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }
}
