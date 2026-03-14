using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Reference { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Supplier { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidatedAt { get; set; }

        public ICollection<ReceiptLine> Lines { get; set; } = new List<ReceiptLine>();
    }

    public class ReceiptLine
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public Receipt Receipt { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal Quantity { get; set; }
    }
}
