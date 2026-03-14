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
    public class ReceiptsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly InventoryService _inv;

        public ReceiptsController(AppDbContext db, InventoryService inv) { _db = db; _inv = inv; }

        public async Task<IActionResult> Index(DocumentStatus? status, int? warehouseId, CancellationToken ct)
        {
            var query = _db.Receipts.Include(r => r.Warehouse).AsQueryable();
            if (status.HasValue) query = query.Where(r => r.Status == status.Value);
            if (warehouseId.HasValue) query = query.Where(r => r.WarehouseId == warehouseId.Value);
            var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync(ct);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name", warehouseId);
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(new Receipt { Reference = "RCP-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Receipt model, int[] productId, decimal[] quantity, CancellationToken ct)
        {
            model.Status = DocumentStatus.Draft;
            _db.Receipts.Add(model);
            await _db.SaveChangesAsync(ct);
            for (var i = 0; i < productId.Length; i++)
            {
                if (quantity[i] <= 0) continue;
                _db.ReceiptLines.Add(new ReceiptLine { ReceiptId = model.Id, ProductId = productId[i], Quantity = quantity[i] });
            }
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var r = await _db.Receipts.Include(x => x.Lines).ThenInclude(l => l.Product).Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (r == null) return NotFound();
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(r);
        }

        [HttpPost]
        public async Task<IActionResult> AddLine(int receiptId, int productId, decimal quantity, CancellationToken ct)
        {
            var r = await _db.Receipts.FindAsync(new object[] { receiptId }, ct);
            if (r == null || r.Status == DocumentStatus.Done) return NotFound();
            _db.ReceiptLines.Add(new ReceiptLine { ReceiptId = receiptId, ProductId = productId, Quantity = quantity });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = receiptId });
        }

        public async Task<IActionResult> Validate(int id, CancellationToken ct)
        {
            var r = await _db.Receipts.FindAsync(new object[] { id }, ct);
            if (r == null) return NotFound();
            _inv.ValidateReceipt(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
