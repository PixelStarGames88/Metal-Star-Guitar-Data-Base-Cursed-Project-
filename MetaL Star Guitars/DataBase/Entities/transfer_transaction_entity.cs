using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetaL_Star_Guitars.DataBase.Entities;

[Table("transfer_transaction", Schema = "public")]
public class transfer_transaction_entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("transfer_transaction_id")]
    public int TransferTransactionId { get; set; }

    [Column("transaction_type")]
    public string TransactionType { get; set; } = null!;

    [Column("issue_date")]
    public DateTime IssueDate { get; set; }

    [Column("transfer_order_id")]
    public int TransferOrderId { get; set; }

    [Column("warehouse_id")]
    public int WarehouseId { get; set; }
}
