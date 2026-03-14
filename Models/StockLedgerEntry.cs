using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class StockLedgerEntry
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DocumentType DocumentType { get; set; }
        public int? DocumentId { get; set; }
        public decimal QuantityIn { get; set; }
        public decimal QuantityOut { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
