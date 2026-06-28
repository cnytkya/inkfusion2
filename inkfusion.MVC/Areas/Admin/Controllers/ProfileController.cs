using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inkfusion.MVC.Data;
using inkfusion.MVC.Models;
using inkfusion.MVC.Utilities;
using inkfusion.MVC.Attributes;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private const string USER_SESSION_KEY = "UserName";
        private const string USER_EMAIL_KEY = "UserEmail";

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString(USER_EMAIL_KEY);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login", new { area = "" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string password = "")
        {
            var userEmail = HttpContext.Session.GetString(USER_EMAIL_KEY);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login", new { area = "" });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Email != userEmail)
            {
                return Unauthorized();
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    user.Name = name;
                }

                if (!string.IsNullOrWhiteSpace(password))
                {
                    if (password.Length < 8)
                    {
                        TempData["Error"] = "Şifre en az 8 karakter olmalıdır.";
                        return View("Index", user);
                    }

                    user.PasswordHash = PasswordHasher.HashPassword(password);
                }

                user.UpdatedAt = DateTime.UtcNow;
                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hata: {ex.Message}";
                return View("Index", user);
            }
        }
    }
}
