using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [MaxLength(200, ErrorMessage = "Maximum 200 characters allowed")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU / Code is required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "Please select category")]
        public int? CategoryId { get; set; }

        public ProductCategory? Category { get; set; }

        [Required(ErrorMessage = "Unit Of Measure is required")]
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = "Unit";

        [Required(ErrorMessage = "Reorder Level is required")]
        [Range(0, 999999, ErrorMessage = "Invalid reorder level")]
        public decimal ReorderLevel { get; set; }

        [Required(ErrorMessage = "Reorder Quantity is required")]
        [Range(0, 999999, ErrorMessage = "Invalid reorder quantity")]
        public decimal ReorderQuantity { get; set; }

        public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
        public ICollection<ReceiptLine> ReceiptLines { get; set; } = new List<ReceiptLine>();
        public ICollection<DeliveryOrderLine> DeliveryOrderLines { get; set; } = new List<DeliveryOrderLine>();
        public ICollection<TransferLine> TransferLines { get; set; } = new List<TransferLine>();
        public ICollection<StockAdjustment> Adjustments { get; set; } = new List<StockAdjustment>();
        public ICollection<StockLedgerEntry> LedgerEntries { get; set; } = new List<StockLedgerEntry>();
    }
}