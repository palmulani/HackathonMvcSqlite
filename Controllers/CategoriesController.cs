using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _db.ProductCategories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync(ct);
            return View(list);
        }

        public IActionResult Create() => View(new ProductCategory());

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory model, CancellationToken ct)
        {
            if (ModelState.IsValid) { _db.ProductCategories.Add(model); await _db.SaveChangesAsync(ct); return RedirectToAction(nameof(Index)); }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var c = await _db.ProductCategories.FindAsync(new object[] { id }, ct);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductCategory model, CancellationToken ct)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid) { _db.ProductCategories.Update(model); await _db.SaveChangesAsync(ct); return RedirectToAction(nameof(Index)); }
            return View(model);
        }
    }
}
