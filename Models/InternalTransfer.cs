using System.ComponentModel.DataAnnotations;

namespace HackathonMvcSqlite.Models
{
    public class InternalTransfer
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Reference { get; set; } = string.Empty;
        public int SourceWarehouseId { get; set; }
        public Warehouse SourceWarehouse { get; set; } = null!;
        public int DestinationWarehouseId { get; set; }
        public Warehouse DestinationWarehouse { get; set; } = null!;
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidatedAt { get; set; }

        public ICollection<TransferLine> Lines { get; set; } = new List<TransferLine>();
    }

    public class TransferLine
    {
        public int Id { get; set; }
        public int InternalTransferId { get; set; }
        public InternalTransfer InternalTransfer { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal Quantity { get; set; }
    }
}
