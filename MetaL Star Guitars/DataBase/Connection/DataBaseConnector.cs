using MetaL_Star_Guitars.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MetaL_Star_Guitars.DataBase.Connection;

class DataBaseConnector : DbContext
{
    // Включаем старое поведение для DateTime, чтобы Postgres не ругался на Kind=Unspecified
    public DataBaseConnector()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<production_order_entity> ProductionOrders { get; set; }
    public DbSet<product_entity> Products { get; set; }
    public DbSet<production_stage_entity> ProductionStages { get; set; }
    public DbSet<product_using_entity> ProductUsings { get; set; }
    public DbSet<route_entity> Routes { get; set; }
    public DbSet<stock_adjustment_document_entity> StockAdjustmentDocuments { get; set; }
    public DbSet<stock_entity> Stocks { get; set; }
    public DbSet<transfer_order_content_entity> TransferOrderContents { get; set; }
    public DbSet<transfer_order_entity> TransferOrders { get; set; }
    public DbSet<transfer_transaction_entity> TransferTransactions { get; set; }
    public DbSet<transit_warehouse_entity> TransitWarehouses { get; set; }
    public DbSet<warehouse_entity> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder
            .UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<product_using_entity>()
            .HasKey(pu => new { pu.ProductId, pu.ProductionStageId });

        modelBuilder.Entity<stock_entity>()
            .HasKey(s => new { s.WarehouseId, s.ProductId });

        modelBuilder.Entity<transfer_order_content_entity>()
            .HasKey(toc => new { toc.TransferOrderId, toc.ProductId });

        base.OnModelCreating(modelBuilder);
    }

    public void CreateProductionOrder(production_order_entity order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        ProductionOrders.Add(order);
        SaveChanges();
    }

    public void CreateStockAdjustmentDocument(stock_adjustment_document_entity document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        StockAdjustmentDocuments.Add(document);
        SaveChanges();
    }

    public void CreateTransferTransaction(transfer_transaction_entity transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        TransferTransactions.Add(transaction);
        SaveChanges();
    }

    public void CreateTransferOrder(transfer_order_entity order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        TransferOrders.Add(order);
        SaveChanges();
    }

    public void CreateTransferOrderContent(transfer_order_content_entity content)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        TransferOrderContents.Add(content);
        SaveChanges();
    }
}