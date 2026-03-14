using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;


        public DashboardController(AppDbContext db) => _db = db;

        //    public async Task<IActionResult> Index(
        //        DocumentType? documentType,
        //        DocumentStatus? status,
        //        int? warehouseId,
        //        int? categoryId,
        //        CancellationToken cancellationToken)
        //    {
        //        var productsWithStock = await _db.StockBalances
        //            .GroupBy(b => b.ProductId)
        //            .Select(g => new
        //            {
        //                ProductId = g.Key,
        //                Total = g.Sum(b => (double)b.Quantity)
        //            })
        //            .ToListAsync(cancellationToken);

        //        var productIdsInStock = productsWithStock
        //            .Where(x => x.Total > 0)
        //            .Select(x => x.ProductId)
        //            .ToHashSet();

        //        var totalProductsInStock = productIdsInStock.Count;

        //        var lowStockList = await (from p in _db.Products
        //                                  where p.ReorderLevel > 0
        //                                  let total = _db.StockBalances
        //                                      .Where(b => b.ProductId == p.Id)
        //                                      .Sum(b => (double)b.Quantity)
        //                                  where total <= (double)p.ReorderLevel
        //                                  select new LowStockProductVm
        //                                  {
        //                                      ProductId = p.Id,
        //                                      Name = p.Name,
        //                                      Sku = p.Sku ?? "",
        //                                      TotalQty = (decimal)total,
        //                                      ReorderLevel = p.ReorderLevel
        //                                  })
        //                                  .Take(10)
        //                                  .ToListAsync(cancellationToken);

        //        var lowOrOut = await _db.Products
        //            .Where(p => p.ReorderLevel > 0 &&
        //                _db.StockBalances
        //                    .Where(b => b.ProductId == p.Id)
        //                    .Sum(b => (double)b.Quantity) <= (double)p.ReorderLevel)
        //            .CountAsync(cancellationToken);

        //        var pendingReceipts = await _db.Receipts
        //            .CountAsync(r => r.Status != DocumentStatus.Done && r.Status != DocumentStatus.Canceled, cancellationToken);

        //        var pendingDeliveries = await _db.DeliveryOrders
        //            .CountAsync(d => d.Status != DocumentStatus.Done && d.Status != DocumentStatus.Canceled, cancellationToken);

        //        var scheduledTransfers = await _db.InternalTransfers
        //            .CountAsync(t => t.Status != DocumentStatus.Done && t.Status != DocumentStatus.Canceled, cancellationToken);

        //        var recentReceipts = await _db.Receipts
        //            .Include(r => r.Warehouse)
        //            .Where(r => documentType == null || documentType == DocumentType.Receipt)
        //            .Where(r => status == null || r.Status == status)
        //            .Where(r => warehouseId == null || r.WarehouseId == warehouseId)
        //            .OrderByDescending(r => r.CreatedAt)
        //            .Take(20)
        //            .Select(r => new DashboardDocumentItem
        //            {
        //                Type = "Receipt",
        //                Reference = r.Reference,
        //                Status = r.Status.ToString(),
        //                Warehouse = r.Warehouse.Name,
        //                CreatedAt = r.CreatedAt
        //            })
        //            .ToListAsync(cancellationToken);

        //        var recentDeliveries = await _db.DeliveryOrders
        //            .Include(d => d.Warehouse)
        //            .Where(d => documentType == null || documentType == DocumentType.Delivery)
        //            .Where(d => status == null || d.Status == status)
        //            .Where(d => warehouseId == null || d.WarehouseId == warehouseId)
        //            .OrderByDescending(d => d.CreatedAt)
        //            .Take(20)
        //            .Select(d => new DashboardDocumentItem
        //            {
        //                Type = "Delivery",
        //                Reference = d.Reference,
        //                Status = d.Status.ToString(),
        //                Warehouse = d.Warehouse.Name,
        //                CreatedAt = d.CreatedAt
        //            })
        //            .ToListAsync(cancellationToken);

        //        var recentTransfers = await _db.InternalTransfers
        //            .Include(t => t.SourceWarehouse)
        //            .Where(t => documentType == null || documentType == DocumentType.Internal)
        //            .Where(t => status == null || t.Status == status)
        //            .Where(t => warehouseId == null || t.SourceWarehouseId == warehouseId)
        //            .OrderByDescending(t => t.CreatedAt)
        //            .Take(20)
        //            .Select(t => new DashboardDocumentItem
        //            {
        //                Type = "Internal Transfer",
        //                Reference = t.Reference,
        //                Status = t.Status.ToString(),
        //                Warehouse = t.SourceWarehouse.Name,
        //                CreatedAt = t.CreatedAt
        //            })
        //            .ToListAsync(cancellationToken);

        //        var allRecent = recentReceipts
        //            .Concat(recentDeliveries)
        //            .Concat(recentTransfers)
        //            .OrderByDescending(x => x.CreatedAt)
        //            .Take(15)
        //            .ToList();

        //        var vm = new DashboardViewModel
        //        {
        //            TotalProductsInStock = totalProductsInStock,
        //            LowStockOrOutOfStockCount = lowOrOut,
        //            PendingReceiptsCount = pendingReceipts,
        //            PendingDeliveriesCount = pendingDeliveries,
        //            InternalTransfersScheduledCount = scheduledTransfers,
        //            RecentDocuments = allRecent,
        //            LowStockProducts = lowStockList
        //        };

        //        ViewBag.Warehouses = await _db.Warehouses
        //            .OrderBy(w => w.Name)
        //            .ToListAsync(cancellationToken);

        //        ViewBag.DocumentType = documentType;
        //        ViewBag.Status = status;
        //        ViewBag.WarehouseId = warehouseId;

        //        return View(vm);
        //    }
        //}

    }
}