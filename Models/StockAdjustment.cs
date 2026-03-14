using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class StockAdjustment
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Reference { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public decimal CountedQuantity { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal Difference => CountedQuantity - PreviousQuantity;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(200)]
        public string? Reason { get; set; }
    }
}
