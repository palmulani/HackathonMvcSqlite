using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Sku { get; set; }

        public int? CategoryId { get; set; }
        public ProductCategory? Category { get; set; }

        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = "Unit";

        public decimal ReorderLevel { get; set; }
        public decimal ReorderQuantity { get; set; }

        public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
        public ICollection<ReceiptLine> ReceiptLines { get; set; } = new List<ReceiptLine>();
        public ICollection<DeliveryOrderLine> DeliveryOrderLines { get; set; } = new List<DeliveryOrderLine>();
        public ICollection<TransferLine> TransferLines { get; set; } = new List<TransferLine>();
        public ICollection<StockAdjustment> Adjustments { get; set; } = new List<StockAdjustment>();
        public ICollection<StockLedgerEntry> LedgerEntries { get; set; } = new List<StockLedgerEntry>();
    }
}
