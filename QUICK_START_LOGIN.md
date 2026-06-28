# Quick Start Guide - Login System

## What Was Implemented

A complete session-based authentication system for your ASP.NET Core 9.0 MVC application with:
- Login form with modern dark UI
- Session-based user authentication
- Admin area protection
- User dropdown menu in navbar
- Logout functionality

## Credentials for Testing

```
Email:    ramaza.ciniogli@gmail.com
Password: Ramazan2026.R
```

## How to Use

### 1. Access Login Page
Navigate to: `https://localhost:5001/Login`

### 2. Login
- Enter email: `ramaza.ciniogli@gmail.com`
- Enter password: `Ramazan2026.R`
- Click "Giriş Yap"

### 3. After Login
You'll be redirected to `/Admin/Dashboard`

You can see:
- Welcome message with your email
- User dropdown menu in navbar (shows your email)
- Logout option in the dropdown

### 4. Logout
- Click the user dropdown in navbar
- Click "Çıkış Yap" (Logout)
- You'll be redirected to home page

## Protected Areas

The following areas now require login:
- `/Admin/Dashboard`
- `/Admin/Artists` (all artist management pages)

Attempting to access without login will automatically redirect to `/Login`

## Files Created

1. **LoginController.cs** - Handles authentication logic
2. **RequireLoginAttribute.cs** - Authorization attribute for protecting pages
3. **Login.cshtml** - Beautiful login form with dark theme
4. **Updated Program.cs** - Added session configuration

## Files Modified

1. **_Layout.cshtml** - Updated navbar with login/logout links
2. **DashboardController.cs** - Added login requirement
3. **ArtistsController.cs** - Added login requirement
4. **Dashboard/Index.cshtml** - Added user welcome message

## Key Features

✓ Session-based authentication (30-minute timeout)
✓ Secure cookies (HttpOnly, SameSite protection)
✓ CSRF protection with anti-forgery tokens
✓ Turkish language interface
✓ Modern responsive UI with dark theme
✓ Error handling and logging
✓ Unauthorized access protection
✓ User dropdown menu with quick links

## Session Behavior

- **Timeout:** 30 minutes of inactivity
- **Session Key:** UserEmail (stores logged-in user's email)
- **Cookie Security:** HttpOnly flag prevents JavaScript access
- **Cross-site Protection:** SameSite=Lax cookie policy

## What Happens When...

| Action | Result |
|--------|--------|
| Click "Giriş Yap" button in navbar | Redirected to `/Login` page |
| Enter wrong credentials | Error message shows, stay on login page |
| Enter correct credentials | Redirected to `/Admin/Dashboard` |
| Click user dropdown | Shows Dashboard, Artists, and Logout options |
| Click "Çıkış Yap" | Session cleared, redirected to home page |
| Try accessing `/Admin/Dashboard` without login | Auto-redirect to `/Login` |
| Stay idle for 30+ minutes | Session expires, redirected to login on next action |

## Production Checklist

Before deploying to production:

- [ ] Change hardcoded credentials
- [ ] Implement database-backed user storage
- [ ] Add proper password hashing (BCrypt, Argon2)
- [ ] Enable HTTPS
- [ ] Set up rate limiting on login attempts
- [ ] Implement account lockout after failed attempts
- [ ] Add email verification
- [ ] Set up password reset functionality
- [ ] Enable security logging and monitoring
- [ ] Consider adding two-factor authentication (2FA)

## Customization Examples

### Change Session Timeout
Edit `Program.cs`:
```csharp
options.IdleTimeout = TimeSpan.FromMinutes(60); // Change to 60 minutes
```

### Change Login Credentials
Edit `LoginController.cs`:
```csharp
private const string VALID_EMAIL = "new.email@example.com";
private const string VALID_PASSWORD = "NewPassword123!";
```

### Protect Another Controller
Add the attribute to your controller:
```csharp
[RequireLogin]
public class MyController : Controller
{
    // All actions require login
}
```

### Protect Specific Actions Only
```csharp
public class MyController : Controller
{
    public IActionResult PublicAction() { } // No login required
    
    [RequireLogin]
    public IActionResult ProtectedAction() { } // Login required
}
```

## Testing Checklist

- [ ] Test login with correct credentials
- [ ] Test login with incorrect credentials
- [ ] Test logout functionality
- [ ] Test that admin pages redirect to login when not authenticated
- [ ] Test that navbar shows correct buttons (login/dropdown)
- [ ] Test responsive design on mobile
- [ ] Test session timeout behavior
- [ ] Test error messages display correctly
- [ ] Test CSRF protection (try submitting form without token)

## Troubleshooting

**Problem:** Login button doesn't redirect to login page
- **Solution:** Clear browser cache and try again

**Problem:** After login, still see "Giriş Yap" button
- **Solution:** Refresh the page (F5)

**Problem:** Can't access Admin panel even when logged in
- **Solution:** Ensure `app.UseSession()` is in Program.cs after routing

**Problem:** Session expires immediately
- **Solution:** Check session timeout configuration in Program.cs

## Support & Documentation

For detailed information, see: `LOGIN_SYSTEM_DOCUMENTATION.md`

---

**Ready to use!** Start by navigating to `/Login` and testing with the provided credentials.
