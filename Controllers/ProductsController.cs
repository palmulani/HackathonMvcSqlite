using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, int? categoryId, CancellationToken ct)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Sku != null && p.Sku.Contains(search)));
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);
            var list = await query.OrderBy(p => p.Name).ToListAsync(ct);
            ViewBag.Categories = new SelectList(await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync(ct), "Id", "Name", categoryId);
            ViewBag.Search = search;
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Categories = new SelectList(await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync(ct), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, decimal? initialStock, int? warehouseId, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(model);
                await _db.SaveChangesAsync(ct);
                if (initialStock.HasValue && initialStock > 0 && warehouseId.HasValue)
                {
                    var inv = HttpContext.RequestServices.GetRequiredService<Services.InventoryService>();
                    inv.AddStock(model.Id, warehouseId.Value, initialStock.Value, DocumentType.Adjustment, null, "Initial stock");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync(ct), "Id", "Name", model.CategoryId);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            return View(model);
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

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var p = await _db.Products.FindAsync(new object[] { id }, ct);
            if (p == null) return NotFound();
            ViewBag.Categories = new SelectList(await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync(ct), "Id", "Name", p.CategoryId);
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product model, CancellationToken ct)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Products.Update(model);
                await _db.SaveChangesAsync(ct);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _db.ProductCategories.OrderBy(c => c.Name).ToListAsync(ct), "Id", "Name", model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> Stock(int id, CancellationToken ct)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);
            if (product == null) return NotFound();
            var balances = await _db.StockBalances
                .Where(b => b.ProductId == id)
                .Include(b => b.Warehouse)
                .Select(b => new { b.Warehouse.Name, b.Quantity })
                .ToListAsync(ct);
            ViewBag.Product = product;
            ViewBag.Balances = balances;
            return View();
        }
    }
}
