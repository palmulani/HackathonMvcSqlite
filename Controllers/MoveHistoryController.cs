using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class MoveHistoryController : Controller
    {
        private readonly AppDbContext _db;

        public MoveHistoryController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(DocumentType? documentType, int? productId, int? warehouseId, CancellationToken ct)
        {
            var query = _db.StockLedgerEntries.Include(e => e.Product).Include(e => e.Warehouse).AsQueryable();
            if (documentType.HasValue) query = query.Where(e => e.DocumentType == documentType.Value);
            if (productId.HasValue) query = query.Where(e => e.ProductId == productId.Value);
            if (warehouseId.HasValue) query = query.Where(e => e.WarehouseId == warehouseId.Value);
            var list = await query.OrderByDescending(e => e.CreatedAt).Take(200).ToListAsync(ct);
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name", productId);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name", warehouseId);
            ViewBag.DocumentType = documentType;
            return View(list);
        }
    }
}
