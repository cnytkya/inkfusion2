using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inkfusion.MVC.Data;
using inkfusion.MVC.Models;
using inkfusion.MVC.Attributes;
using inkfusion.MVC.Utilities;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [RequireLogin]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Admin/Users
        /// List all users
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcılar yüklenirken bir hata oluştu.";
                return View(new List<User>());
            }
        }

        /// <summary>
        /// GET: /Admin/Users/Create
        /// Display form to create new user
        /// </summary>
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: /Admin/Users/Create
        /// Create new user
        /// </summary>
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string email, string password, string confirmPassword)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("name", "Ad alanı boş olamaz.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("email", "E-posta alanı boş olamaz.");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("password", "Şifre alanı boş olamaz.");
                }

                if (password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Şifreler eşleşmiyor.");
                }

                if (password?.Length < 8)
                {
                    ModelState.AddModelError("password", "Şifre en az 8 karakter uzunluğunda olmalıdır.");
                }

                // Check if email already exists
                if (!string.IsNullOrWhiteSpace(email) && await _context.Users.AnyAsync(u => u.Email == email))
                {
                    ModelState.AddModelError("email", "Bu e-posta adresi zaten kullanılmaktadır.");
                }

                if (!ModelState.IsValid)
                {
                    return View();
                }

                // Create new user with hashed password
                var user = new User
                {
                    Name = name?.Trim() ?? string.Empty,
                    Email = email?.Trim() ?? string.Empty,
                    PasswordHash = PasswordHasher.HashPassword(password!),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.Email}' created successfully.");
                TempData["SuccessMessage"] = $"'{user.Name}' kullanıcısı başarıyla oluşturuldu.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı oluşturulurken bir hata oluştu.";
                return View();
            }
        }

        /// <summary>
        /// GET: /Admin/Users/Edit/5
        /// Display form to edit user
        /// </summary>
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı yüklenirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Admin/Users/Edit/5
        /// Update user
        /// </summary>
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string email, string? password, string? confirmPassword, bool isActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                // Validation
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("name", "Ad alanı boş olamaz.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("email", "E-posta alanı boş olamaz.");
                }

                // Check if email is unique (excluding current user)
                if (!string.IsNullOrWhiteSpace(email) && email != user.Email &&
                    await _context.Users.AnyAsync(u => u.Email == email))
                {
                    ModelState.AddModelError("email", "Bu e-posta adresi zaten kullanılmaktadır.");
                }

                // Validate password if provided
                if (!string.IsNullOrWhiteSpace(password))
                {
                    if (password != confirmPassword)
                    {
                        ModelState.AddModelError("confirmPassword", "Şifreler eşleşmiyor.");
                    }

                    if (password.Length < 8)
                    {
                        ModelState.AddModelError("password", "Şifre en az 8 karakter uzunluğunda olmalıdır.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                // Update user
                user.Name = name?.Trim() ?? string.Empty;
                user.Email = email?.Trim() ?? string.Empty;
                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.PasswordHash = PasswordHasher.HashPassword(password);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.Email}' updated successfully.");
                TempData["SuccessMessage"] = $"'{user.Name}' kullanıcısı başarıyla güncellendi.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı güncellenirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Admin/Users/Delete/5
        /// Delete user (soft delete by deactivating)
        /// </summary>
        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                // Prevent deleting the last active admin
                var activeUserCount = await _context.Users.CountAsync(u => u.IsActive);
                if (activeUserCount <= 1 && user.IsActive)
                {
                    TempData["ErrorMessage"] = "Son aktif kullanıcı silinemez.";
                    return RedirectToAction("Index");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.Email}' deleted successfully.");
                TempData["SuccessMessage"] = $"'{user.Name}' kullanıcısı başarıyla silindi.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Admin/Users/Deactivate/5
        /// Deactivate user
        /// </summary>
        [HttpPost]
        [Route("Deactivate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                // Prevent deactivating the last active user
                var activeUserCount = await _context.Users.CountAsync(u => u.IsActive);
                if (activeUserCount <= 1 && user.IsActive)
                {
                    TempData["ErrorMessage"] = "Son aktif kullanıcı devre dışı bırakılamaz.";
                    return RedirectToAction("Index");
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.Email}' deactivated successfully.");
                TempData["SuccessMessage"] = $"'{user.Name}' kullanıcısı devre dışı bırakıldı.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating user {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı devre dışı bırakılırken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Admin/Users/Activate/5
        /// Activate user
        /// </summary>
        [HttpPost]
        [Route("Activate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.Email}' activated successfully.");
                TempData["SuccessMessage"] = $"'{user.Name}' kullanıcısı etkinleştirildi.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error activating user {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Kullanıcı etkinleştirilirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
    }
}
