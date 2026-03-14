using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<StockBalance> StockBalances => Set<StockBalance>();
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<ReceiptLine> ReceiptLines => Set<ReceiptLine>();
        public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();
        public DbSet<DeliveryOrderLine> DeliveryOrderLines => Set<DeliveryOrderLine>();
        public DbSet<InternalTransfer> InternalTransfers => Set<InternalTransfer>();
        public DbSet<TransferLine> TransferLines => Set<TransferLine>();
        public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
        public DbSet<StockLedgerEntry> StockLedgerEntries => Set<StockLedgerEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockBalance>()
                .HasIndex(b => new { b.ProductId, b.WarehouseId })
                .IsUnique();

            modelBuilder.Entity<InternalTransfer>()
                .HasOne(t => t.SourceWarehouse)
                .WithMany(w => w.TransfersFrom)
                .HasForeignKey(t => t.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InternalTransfer>()
                .HasOne(t => t.DestinationWarehouse)
                .WithMany(w => w.TransfersTo)
                .HasForeignKey(t => t.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockAdjustment>()
                .Ignore(a => a.Difference);
        }
    }
}
