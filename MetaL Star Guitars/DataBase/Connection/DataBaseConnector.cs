using MetaL_Star_Guitars.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MetaL_Star_Guitars.DataBase.Connection;

class DataBaseConnector : DbContext
{
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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Указываем путь к текущей папке
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);
    }
}