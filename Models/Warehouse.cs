using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        public string? Code { get; set; }

        public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
        public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
        public ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();
        public ICollection<InternalTransfer> TransfersFrom { get; set; } = new List<InternalTransfer>();
        public ICollection<InternalTransfer> TransfersTo { get; set; } = new List<InternalTransfer>();
        public ICollection<StockAdjustment> Adjustments { get; set; } = new List<StockAdjustment>();
        public ICollection<StockLedgerEntry> LedgerEntries { get; set; } = new List<StockLedgerEntry>();
    }
}
