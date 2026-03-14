using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class WarehousesController : Controller
    {
        private readonly AppDbContext _db;

        public WarehousesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(ct);
            return View(list);
        }

        public IActionResult Create() => View(new Warehouse());

        [HttpPost]
        public async Task<IActionResult> Create(Warehouse model, CancellationToken ct)
        {
            if (ModelState.IsValid) { _db.Warehouses.Add(model); await _db.SaveChangesAsync(ct); return RedirectToAction(nameof(Index)); }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var w = await _db.Warehouses.FindAsync(new object[] { id }, ct);
            if (w == null) return NotFound();
            return View(w);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Warehouse model, CancellationToken ct)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid) { _db.Warehouses.Update(model); await _db.SaveChangesAsync(ct); return RedirectToAction(nameof(Index)); }
            return View(model);
        }
    }
}
