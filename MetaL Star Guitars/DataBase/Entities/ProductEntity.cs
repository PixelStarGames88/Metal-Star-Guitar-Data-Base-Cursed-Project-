using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("product", Schema = "public")]
public class ProductEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    public string ProductName { get; set; } = null!;

    [Column("price")]
    public decimal Price { get; set; }
}