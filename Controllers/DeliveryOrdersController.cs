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
    public class DeliveryOrdersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly InventoryService _inv;

        public DeliveryOrdersController(AppDbContext db, InventoryService inv) { _db = db; _inv = inv; }

        public async Task<IActionResult> Index(DocumentStatus? status, int? warehouseId, CancellationToken ct)
        {
            var query = _db.DeliveryOrders.Include(d => d.Warehouse).AsQueryable();
            if (status.HasValue) query = query.Where(d => d.Status == status.Value);
            if (warehouseId.HasValue) query = query.Where(d => d.WarehouseId == warehouseId.Value);
            var list = await query.OrderByDescending(d => d.CreatedAt).ToListAsync(ct);
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name", warehouseId);
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Warehouses = new SelectList(await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct), "Id", "Name");
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(new DeliveryOrder { Reference = "DO-" + DateTime.UtcNow.ToString("yyyyMMddHHmm") });
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeliveryOrder model, int[] productId, decimal[] quantity, CancellationToken ct)
        {
            model.Status = DocumentStatus.Draft;
            _db.DeliveryOrders.Add(model);
            await _db.SaveChangesAsync(ct);
            for (var i = 0; i < productId.Length; i++)
            {
                if (quantity[i] <= 0) continue;
                _db.DeliveryOrderLines.Add(new DeliveryOrderLine { DeliveryOrderId = model.Id, ProductId = productId[i], Quantity = quantity[i] });
            }
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var d = await _db.DeliveryOrders.Include(x => x.Lines).ThenInclude(l => l.Product).Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (d == null) return NotFound();
            ViewBag.Products = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(ct), "Id", "Name");
            return View(d);
        }

        [HttpPost]
        public async Task<IActionResult> AddLine(int deliveryOrderId, int productId, decimal quantity, CancellationToken ct)
        {
            var d = await _db.DeliveryOrders.FindAsync(new object[] { deliveryOrderId }, ct);
            if (d == null || d.Status == DocumentStatus.Done) return NotFound();
            _db.DeliveryOrderLines.Add(new DeliveryOrderLine { DeliveryOrderId = deliveryOrderId, ProductId = productId, Quantity = quantity });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Edit), new { id = deliveryOrderId });
        }

        public async Task<IActionResult> Validate(int id, CancellationToken ct)
        {
            var d = await _db.DeliveryOrders.FindAsync(new object[] { id }, ct);
            if (d == null) return NotFound();
            _inv.ValidateDeliveryOrder(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
