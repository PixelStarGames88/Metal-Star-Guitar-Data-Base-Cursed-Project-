using Microsoft.EntityFrameworkCore;
using MetaL_Star_Guitars.DataBase.Entities;

namespace MetaL_Star_Guitars.DataBase.Connection;

class DataBaseConnector : DbContext
{
    private readonly string _databaseConnectionString = "Host=localhost;Port=5432;DataBase=postgres;Username=postgres;Password=1309201911112019";
    public DbSet<ProductionOrderEntity> ProductionOrders { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductionStageEntity> ProductionStages { get; set; }
    public DbSet<ProductUsingEntity> ProductUsings { get; set; }
    public DbSet<RouteEntity> Routes { get; set; }
    public DbSet<StockAdjustmentDocumentEntity> StockAdjustmentDocuments { get; set; }
    public DbSet<StockEntity> Stocks { get; set; }
    public DbSet<TransferOrderContentEntity> TransferOrderContents { get; set; }
    public DbSet<TransferOrderEntity> TransferOrders { get; set; }
    public DbSet<TransferTransactionEntity> TransferTransactions { get; set; }
    public DbSet<TransitWarehouseEntity> TransitWarehouses { get; set; }
    public DbSet<WarehouseEntity> Warehouses { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_databaseConnectionString);
    }
}