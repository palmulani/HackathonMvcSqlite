using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reference is required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string Reference { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Maximum 200 characters allowed")]
        public string? Supplier { get; set; }

        [Required(ErrorMessage = "Please select warehouse")]
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

        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.01, 999999, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
    }
}