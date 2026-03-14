using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;
using HackathonMvcSqlite.Services;

namespace HackathonMvcSqlite.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email not found");
                return View();
            }

            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await _db.SaveChangesAsync();

            await _emailService.SendOtp(email, otp);

            TempData["email"] = email;
            TempData["OtpAttempts"] = 0;

            return RedirectToAction("VerifyOtp");
        }
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            var email = TempData["email"]?.ToString();
            TempData.Keep("email");

            if (email == null)
                return RedirectToAction("Login");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login");

            int attempts = TempData["OtpAttempts"] == null ? 0 : (int)TempData["OtpAttempts"];

            if (user.ResetOtp != otp || user.ResetOtpExpiry < DateTime.UtcNow)
            {
                attempts++;
                TempData["OtpAttempts"] = attempts;
                TempData.Keep("OtpAttempts");

                if (attempts >= 3)
                {
                    TempData.Remove("email");
                    TempData.Remove("OtpAttempts");
                    ModelState.AddModelError("", "Too many invalid attempts. Please login again.");
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", $"Invalid OTP. Attempts left: {3 - attempts}");
                return View();
            }

            // OTP correct
            TempData.Remove("OtpAttempts");
            TempData["resetEmail"] = email;

            return RedirectToAction("ResetPassword");
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string password)
        {
            var email = TempData["resetEmail"]?.ToString();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login");

            user.PasswordHash = HashPassword(password);
            user.ResetOtp = null;
            user.ResetOtpExpiry = null;

            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        private static string HashPassword(string password) => Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        private static bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;
    }
}
