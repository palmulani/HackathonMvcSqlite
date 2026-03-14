namespace HackathonMvcSqlite.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProductsInStock { get; set; }
        public int LowStockOrOutOfStockCount { get; set; }
        public int PendingReceiptsCount { get; set; }
        public int PendingDeliveriesCount { get; set; }
        public int InternalTransfersScheduledCount { get; set; }
        public List<DashboardDocumentItem> RecentDocuments { get; set; } = new();
        public List<LowStockProductVm> LowStockProducts { get; set; } = new();
    }

    public class DashboardDocumentItem
    {
        public string Type { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class LowStockProductVm
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal TotalQty { get; set; }
        public decimal ReorderLevel { get; set; }
    }
}
