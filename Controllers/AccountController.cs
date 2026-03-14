using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
            var claims = new List<Claim> { new(ClaimTypes.Email, user.Email), new(ClaimTypes.Name, user.Name), new(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, CancellationToken ct)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            {
                ModelState.AddModelError("", "Email already registered.");
                return View();
            }
            _db.Users.Add(new User { Name = name, Email = email, PasswordHash = HashPassword(password) });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Login));
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private static string HashPassword(string password) => Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        private static bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;
    }
}
