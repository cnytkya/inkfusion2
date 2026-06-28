using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inkfusion.MVC.Data;
using inkfusion.MVC.Utilities;

namespace inkfusion.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;

        // Session key for user name
        private const string USER_SESSION_KEY = "UserName";
        // Session key for user email (for reference)
        private const string USER_EMAIL_SESSION_KEY = "UserEmail";

        public LoginController(ILogger<LoginController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// GET: /Login
        /// Displays the login form
        /// </summary>
        [HttpGet]
        [Route("/Login")]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to admin dashboard
            if (User.Identity?.IsAuthenticated ?? false || IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return View();
        }

        /// <summary>
        /// POST: /Login
        /// Handles login form submission with database verification
        /// </summary>
        [HttpPost]
        [Route("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["ErrorMessage"] = "E-posta ve şifre alanları boş olamaz.";
                    return View();
                }

                // Verify credentials from database
                var user = await VerifyCredentialsAsync(email, password);

                if (user != null)
                {
                    // Set session with user's name (not email)
                    HttpContext.Session.SetString(USER_SESSION_KEY, user.Name);
                    // Also store email for reference
                    HttpContext.Session.SetString(USER_EMAIL_SESSION_KEY, user.Email);

                    _logger.LogInformation($"User {user.Email} ({user.Name}) logged in successfully at {DateTime.UtcNow}");

                    // Redirect to admin dashboard
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {
                    _logger.LogWarning($"Failed login attempt for email: {email} at {DateTime.UtcNow}");
                    TempData["ErrorMessage"] = "E-posta veya şifre hatalı. Lütfen tekrar deneyiniz.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                TempData["ErrorMessage"] = "Giriş sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                return View();
            }
        }

        /// <summary>
        /// GET: /Logout
        /// Logs out the current user
        /// </summary>
        [HttpGet]
        [Route("/Logout")]
        public IActionResult Logout()
        {
            try
            {
                var userName = HttpContext.Session.GetString(USER_SESSION_KEY);
                var userEmail = HttpContext.Session.GetString(USER_EMAIL_SESSION_KEY);

                if (!string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogInformation($"User {userEmail} ({userName}) logged out at {DateTime.UtcNow}");
                }

                // Clear session
                HttpContext.Session.Remove(USER_SESSION_KEY);
                HttpContext.Session.Remove(USER_EMAIL_SESSION_KEY);
                HttpContext.Session.Clear();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during logout: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Verifies if the provided email and password match a user in the database
        /// Returns the user object if credentials are valid, null otherwise
        /// </summary>
        private async Task<Models.User?> VerifyCredentialsAsync(string email, string password)
        {
            try
            {
                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

                if (user == null)
                {
                    return null;
                }

                // Verify password using PasswordHasher utility
                if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying credentials: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if user is currently logged in via session
        /// </summary>
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString(USER_SESSION_KEY));
        }

        /// <summary>
        /// Gets the currently logged-in user's name from session
        /// </summary>
        public string? GetCurrentUserName()
        {
            return HttpContext.Session.GetString(USER_SESSION_KEY);
        }

        /// <summary>
        /// Gets the currently logged-in user's email from session
        /// </summary>
        public string? GetCurrentUserEmail()
        {
            return HttpContext.Session.GetString(USER_EMAIL_SESSION_KEY);
        }
    }
}
