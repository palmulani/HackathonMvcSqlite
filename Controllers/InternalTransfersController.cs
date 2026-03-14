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
    public class InternalTransfersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly InventoryService _inv;

        public InternalTransfersController(AppDbContext db, InventoryService inv) { _db = db; _inv = inv; }

        public async Task<IActionResult> Index(DocumentStatus? status, int? warehouseId, CancellationToken ct)
        {
            var query = _db.InternalTransfers.Include(t => t.SourceWarehouse).Include(t => t.DestinationWarehouse).AsQueryable();
            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (warehouseId.HasValue) query = query.Where(t => t.SourceWarehouseId == warehouseId.Value || t.DestinationWarehouseId == warehouseId.Value);
            var list = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name", warehouseId);
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(new InternalTransfer { Reference = "TRF-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") });
        }

        [HttpPost]
        public async Task<IActionResult> Create(InternalTransfer model, int[] productId, decimal[] quantity, CancellationToken ct)
        {
            if (model.SourceWarehouseId == model.DestinationWarehouseId) { ModelState.AddModelError("", "Source and destination must differ."); return View(model); }
            model.Status = DocumentStatus.Draft;
            _db.InternalTransfers.Add(model);
            await _db.SaveChangesAsync(ct);
            for (var i = 0; i < productId.Length; i++)
            {
                if (quantity[i] <= 0) continue;
                _db.TransferLines.Add(new TransferLine { InternalTransferId = model.Id, ProductId = productId[i], Quantity = quantity[i] });
            }
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
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
            var t = await _db.InternalTransfers.Include(x => x.Lines).ThenInclude(l => l.Product).Include(x => x.SourceWarehouse).Include(x => x.DestinationWarehouse).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (t == null) return NotFound();
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(t);
        }

        [HttpPost]
        public async Task<IActionResult> AddLine(int internalTransferId, int productId, decimal quantity, CancellationToken ct)
        {
            var t = await _db.InternalTransfers.FindAsync(new object[] { internalTransferId }, ct);
            if (t == null || t.Status == DocumentStatus.Done) return NotFound();
            _db.TransferLines.Add(new TransferLine { InternalTransferId = internalTransferId, ProductId = productId, Quantity = quantity });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = internalTransferId });
        }

        public async Task<IActionResult> Validate(int id, CancellationToken ct)
        {
            var t = await _db.InternalTransfers.FindAsync(new object[] { id }, ct);
            if (t == null) return NotFound();
            _inv.ValidateInternalTransfer(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
