# Code Snippets Reference - Login System Customization

This document provides ready-to-use code snippets for common customization tasks.

## 1. Protect a New Controller with Login

Add the `[RequireLogin]` attribute above any controller that requires authentication:

```csharp
using inkfusion.MVC.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin]  // Add this line
    public class MyNewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```

## 2. Protect Individual Actions Only

You can also protect specific actions while keeping others public:

```csharp
[RequireLogin]
public class MyController : Controller
{
    // This action is public (no login required)
    public IActionResult PublicAction()
    {
        return View();
    }

    // This action requires login
    [RequireLogin]
    public IActionResult ProtectedAction()
    {
        return View();
    }
}
```

## 3. Get Current Logged-In User Email in Controller

```csharp
public class MyController : Controller
{
    public IActionResult MyAction()
    {
        // Get user email from session
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userEmail != null)
        {
            ViewBag.CurrentUser = userEmail;
            // User is logged in
        }

        return View();
    }
}
```

## 4. Check if User is Logged In in View

```html
@{
    var userEmail = Context.Session.GetString("UserEmail");
}

@if (!string.IsNullOrEmpty(userEmail))
{
    <p>Welcome, @userEmail!</p>
    <a href="/Logout">Logout</a>
}
else
{
    <a href="/Login">Login</a>
}
```

## 5. Change Session Timeout

Edit `Program.cs`:

```csharp
// Current: 30 minutes
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Change this value
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Examples:
// 15 minutes:  TimeSpan.FromMinutes(15)
// 1 hour:      TimeSpan.FromHours(1)
// 8 hours:     TimeSpan.FromHours(8)
```

## 6. Change Login Credentials

Edit `LoginController.cs`:

```csharp
public class LoginController : Controller
{
    // Change these constants to new credentials
    private const string VALID_EMAIL = "new.email@example.com";
    private const string VALID_PASSWORD = "NewPassword123!";

    // Rest of code...
}
```

## 7. Add Current User to Page Title

In any view that displays user information:

```html
@{
    var userEmail = Context.Session.GetString("UserEmail");
}

<h1>Welcome, @userEmail</h1>
```

In controller:

```csharp
public IActionResult Index()
{
    var userEmail = HttpContext.Session.GetString("UserEmail");
    ViewBag.PageTitle = $"Dashboard - {userEmail}";
    return View();
}
```

## 8. Add Conditional CSS Class Based on Login Status

In `_Layout.cshtml`:

```html
@{
    var isLoggedIn = !string.IsNullOrEmpty(Context.Session.GetString("UserEmail"));
}

<body class="@(isLoggedIn ? "logged-in" : "logged-out")">
    <!-- Your content -->
</body>
```

Then in CSS:

```css
body.logged-in {
    /* Styles for logged-in users */
}

body.logged-out {
    /* Styles for logged-out users */
}
```

## 9. Log User Actions

In any controller where you want to track user actions:

```csharp
private readonly ILogger<MyController> _logger;

public MyController(ILogger<MyController> logger)
{
    _logger = logger;
}

public IActionResult MyAction()
{
    var userEmail = HttpContext.Session.GetString("UserEmail");
    _logger.LogInformation($"User {userEmail} accessed MyAction at {DateTime.UtcNow}");

    return View();
}
```

## 10. Redirect to Login with Return URL

In any controller, save the current URL before redirecting:

```csharp
[HttpGet]
public IActionResult AccessDenied()
{
    HttpContext.Session.SetString("ReturnUrl", HttpContext.Request.Path);
    return RedirectToAction("Login", "Login");
}
```

Then after login, redirect back:

```csharp
[HttpPost]
public IActionResult Login(string email, string password)
{
    if (VerifyCredentials(email, password))
    {
        HttpContext.Session.SetString("UserEmail", email);

        // Redirect to return URL if exists
        var returnUrl = HttpContext.Session.GetString("ReturnUrl");
        if (!string.IsNullOrEmpty(returnUrl))
        {
            HttpContext.Session.Remove("ReturnUrl");
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    return View();
}
```

## 11. Create a Helper Method for User Info

Add to your controller base class or utility class:

```csharp
public static class SessionHelper
{
    private const string USER_SESSION_KEY = "UserEmail";

    public static string? GetCurrentUser(HttpContext httpContext)
    {
        return httpContext.Session.GetString(USER_SESSION_KEY);
    }

    public static bool IsUserLoggedIn(HttpContext httpContext)
    {
        return !string.IsNullOrEmpty(GetCurrentUser(httpContext));
    }

    public static void SetUserSession(HttpContext httpContext, string email)
    {
        httpContext.Session.SetString(USER_SESSION_KEY, email);
    }

    public static void ClearUserSession(HttpContext httpContext)
    {
        httpContext.Session.Remove(USER_SESSION_KEY);
        httpContext.Session.Clear();
    }
}
```

Usage:

```csharp
var currentUser = SessionHelper.GetCurrentUser(HttpContext);
if (SessionHelper.IsUserLoggedIn(HttpContext))
{
    // User is logged in
}
```

## 12. Add Remember Me Functionality (Future)

In `LoginController.cs`:

```csharp
[HttpPost]
public IActionResult Login(string email, string password, bool rememberMe)
{
    if (VerifyCredentials(email, password))
    {
        HttpContext.Session.SetString("UserEmail", email);

        if (rememberMe)
        {
            // Save persistent cookie (future enhancement)
            Response.Cookies.Append("RememberedUser", email, 
                new CookieOptions 
                { 
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    return View();
}
```

## 13. Add Login Attempt Counter (Future Enhancement)

```csharp
public class LoginAttemptTracker
{
    private static Dictionary<string, (int attempts, DateTime lockoutTime)> _attempts = 
        new Dictionary<string, (int, DateTime)>();

    private const int MAX_ATTEMPTS = 5;
    private static readonly TimeSpan LOCKOUT_DURATION = TimeSpan.FromMinutes(15);

    public static bool IsLockedOut(string email)
    {
        if (_attempts.TryGetValue(email, out var attempt))
        {
            if (attempt.attempts >= MAX_ATTEMPTS)
            {
                if (DateTime.UtcNow - attempt.lockoutTime < LOCKOUT_DURATION)
                {
                    return true;
                }
                else
                {
                    // Lockout period expired
                    _attempts.Remove(email);
                    return false;
                }
            }
        }

        return false;
    }

    public static void RecordFailedAttempt(string email)
    {
        if (_attempts.TryGetValue(email, out var attempt))
        {
            if (DateTime.UtcNow - attempt.lockoutTime > LOCKOUT_DURATION)
            {
                _attempts[email] = (1, DateTime.UtcNow);
            }
            else
            {
                _attempts[email] = (attempt.attempts + 1, attempt.lockoutTime);
            }
        }
        else
        {
            _attempts[email] = (1, DateTime.UtcNow);
        }
    }

    public static void ClearAttempts(string email)
    {
        _attempts.Remove(email);
    }
}
```

## 14. Add Email Validation

In `LoginController.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

private bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}

[HttpPost]
public IActionResult Login(string email, string password)
{
    if (!IsValidEmail(email))
    {
        TempData["ErrorMessage"] = "Geçersiz e-posta formatı.";
        return View();
    }

    // Rest of login logic...
}
```

## 15. Add Password Strength Validation (Future)

```csharp
private bool IsStrongPassword(string password)
{
    // At least 8 characters
    if (password.Length < 8)
        return false;

    // Contains uppercase
    if (!password.Any(char.IsUpper))
        return false;

    // Contains lowercase
    if (!password.Any(char.IsLower))
        return false;

    // Contains digit
    if (!password.Any(char.IsDigit))
        return false;

    // Contains special character
    if (!password.Any(c => !char.IsLetterOrDigit(c)))
        return false;

    return true;
}

// Usage in login controller
if (!IsStrongPassword(password))
{
    TempData["ErrorMessage"] = "Şifre en az 8 karakter içermeli, büyük harf, küçük harf, rakam ve özel karakter içermelidir.";
    return View();
}
```

## 16. Automatic Logout on Session Expiry

Add to `_Layout.cshtml`:

```html
<script>
    // Check session status every minute
    setInterval(function() {
        fetch('/api/session/check')
            .then(response => response.json())
            .then(data => {
                if (!data.isActive) {
                    // Session expired
                    alert('Oturumunuz sona erdi. Lütfen tekrar giriş yapınız.');
                    window.location.href = '/Login';
                }
            })
            .catch(err => console.error('Session check failed:', err));
    }, 60000); // Check every 60 seconds
</script>
```

## 17. Display Logout Confirmation Dialog

In `_Layout.cshtml`:

```html
<a href="#" onclick="return confirmLogout()">Logout</a>

<script>
    function confirmLogout() {
        if (confirm('Çıkış yapmak istediğinizden emin misiniz?')) {
            window.location.href = '/Logout';
        }
        return false;
    }
</script>
```

## 18. Add User Profile Section

Create new view: `Views/Account/Profile.cshtml`

```html
@{
    var userEmail = Context.Session.GetString("UserEmail");
}

<div class="profile-container">
    <h1>Profil Bilgileri</h1>
    <p>Email: @userEmail</p>
    <p>Son Giriş: @DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss")</p>
    <a href="/Logout" class="btn btn-danger">Çıkış Yap</a>
</div>
```

Then add controller:

```csharp
[RequireLogin]
public class AccountController : Controller
{
    public IActionResult Profile()
    {
        return View();
    }
}
```

## 19. Add Login Statistics to Dashboard

```csharp
public IActionResult Index()
{
    var userEmail = HttpContext.Session.GetString("UserEmail");
    
    var statistics = new
    {
        LoginTime = DateTime.UtcNow,
        UserEmail = userEmail,
        SessionTimeout = TimeSpan.FromMinutes(30),
        IsLoggedIn = !string.IsNullOrEmpty(userEmail)
    };

    ViewBag.Statistics = statistics;
    return View();
}
```

## 20. Create Custom Authorization Error Page

Create view: `Views/Auth/Unauthorized.cshtml`

```html
@{
    ViewData["Title"] = "Yetkisiz Erişim";
}

<div class="alert alert-danger mt-5">
    <h4>Yetkisiz Erişim</h4>
    <p>Bu sayfaya erişim izniniz yoktur. Lütfen giriş yapınız.</p>
    <a href="/Login" class="btn btn-primary">Giriş Sayfasına Dön</a>
    <a href="/" class="btn btn-secondary">Ana Sayfaya Dön</a>
</div>
```

Update `RequireLoginAttribute.cs`:

```csharp
context.Result = new RedirectToRouteResult(
    new RouteValueDictionary
    {
        { "controller", "Auth" },
        { "action", "Unauthorized" }
    });
```

---

## Tips & Best Practices

1. Always validate user input on server-side
2. Use HTTPS in production
3. Never log passwords
4. Clear sensitive data from memory
5. Use constant-time comparison for security-critical values
6. Implement rate limiting on login endpoint
7. Keep session timeout reasonable (15-30 minutes for admin, longer for public)
8. Use HttpOnly cookies for session storage
9. Log all authentication events
10. Regularly review security logs

---

For more information, refer to the main documentation files in the project root.
