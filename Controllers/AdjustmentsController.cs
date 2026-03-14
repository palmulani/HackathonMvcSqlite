using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;
using HackathonMvcSqlite.Services;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class AdjustmentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly InventoryService _inv;

        public AdjustmentsController(AppDbContext db, InventoryService inv) { _db = db; _inv = inv; }

        public async Task<IActionResult> Index(int? warehouseId, CancellationToken ct)
        {
            var query = _db.StockAdjustments.Include(a => a.Product).Include(a => a.Warehouse).AsQueryable();
            if (warehouseId.HasValue) query = query.Where(a => a.WarehouseId == warehouseId.Value);
            var list = await query.OrderByDescending(a => a.CreatedAt).ToListAsync(ct);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name", warehouseId);
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            return View(new StockAdjustment { Reference = "ADJ-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") });
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockAdjustment model, CancellationToken ct)
        {
            model.PreviousQuantity = _inv.GetStock(model.ProductId, model.WarehouseId);
            _inv.ApplyAdjustment(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var category = await _db.ProductCategories.FindAsync(new object[] { id }, ct);

            if (category != null)
            {
                _db.ProductCategories.Remove(category);
                await _db.SaveChangesAsync(ct);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
