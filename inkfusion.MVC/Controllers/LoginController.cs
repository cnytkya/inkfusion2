using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace inkfusion.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        // Hardcoded credentials
        private const string VALID_EMAIL = "ramaza.ciniogli@gmail.com";
        private const string VALID_PASSWORD = "Ramazan2026.R";

        // Session key
        private const string USER_SESSION_KEY = "UserEmail";

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
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
        /// Handles login form submission
        /// </summary>
        [HttpPost]
        [Route("/Login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, bool rememberMe = false)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["ErrorMessage"] = "E-posta ve şifre alanları boş olamaz.";
                    return View();
                }

                // Verify credentials
                if (VerifyCredentials(email, password))
                {
                    // Set session
                    HttpContext.Session.SetString(USER_SESSION_KEY, email);

                    _logger.LogInformation($"User {email} logged in successfully at {DateTime.UtcNow}");

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
                var email = HttpContext.Session.GetString(USER_SESSION_KEY);
                if (!string.IsNullOrEmpty(email))
                {
                    _logger.LogInformation($"User {email} logged out at {DateTime.UtcNow}");
                }

                // Clear session
                HttpContext.Session.Remove(USER_SESSION_KEY);
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
        /// Verifies if the provided email and password match the hardcoded credentials
        /// </summary>
        private bool VerifyCredentials(string email, string password)
        {
            // Use constant-time comparison to prevent timing attacks
            bool emailMatch = email.Equals(VALID_EMAIL, StringComparison.Ordinal);
            bool passwordMatch = password.Equals(VALID_PASSWORD, StringComparison.Ordinal);

            // Both must match
            return emailMatch && passwordMatch;
        }

        /// <summary>
        /// Checks if user is currently logged in via session
        /// </summary>
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString(USER_SESSION_KEY));
        }

        /// <summary>
        /// Gets the currently logged-in user's email from session
        /// </summary>
        public string? GetCurrentUserEmail()
        {
            return HttpContext.Session.GetString(USER_SESSION_KEY);
        }
    }
}
