# User Management System - Quick Start Guide

## Login Credentials

### Seeded Admin User
- **Email**: ramaza.ciniogli@gmail.com
- **Password**: Ramazan2026.R
- **Name**: Ramazan Cinioglu

## Access Points

### Public Login
- **URL**: `http://localhost:5000/Login`
- **Method**: Enter email and password
- **Result**: Redirects to Admin Dashboard

### Admin Panel
- **URL**: `http://localhost:5000/Admin/Dashboard`
- **Access**: Requires login (auto-redirect if not authenticated)

### User Management
- **URL**: `http://localhost:5000/Admin/Users`
- **Access**: Requires login
- **Features**: Add, edit, delete, activate/deactivate users

## Common Tasks

### Login to System
1. Go to `/Login`
2. Email: `ramaza.ciniogli@gmail.com`
3. Password: `Ramazan2026.R`
4. Click login

### Create New User
1. Login to system
2. Go to Admin → Kullanıcılar (Users)
3. Click "Yeni Kullanıcı Ekle" (Add New User)
4. Enter:
   - Name: Full name
   - Email: Unique email
   - Password: Minimum 8 characters
   - Confirm Password: Same as above
5. Click "Kaydet" (Save)

### Edit User
1. Go to Users list
2. Click edit button (pencil icon)
3. Modify name, email, or password
4. Toggle Active status if needed
5. Click "Kaydet" (Save)

### Deactivate User
1. Go to Users list
2. Click deactivate button (ban icon)
3. Confirm action
4. User will be unable to login

### Delete User
1. Go to Users list
2. Click delete button (trash icon)
3. Confirm deletion
4. User is permanently removed

### Logout
1. Click user dropdown (top right)
2. Click "Çıkış Yap" (Logout)
3. Session cleared, redirected to home

## Database

### Tables
- `Artists` - Existing artists table
- `Users` - New users table (with seeded admin)

### Users Table Columns
- `Id` - Primary key
- `Name` - User's full name
- `Email` - Unique email address
- `PasswordHash` - BCrypt hashed password
- `IsActive` - Account status (true/false)
- `CreatedAt` - Creation timestamp
- `UpdatedAt` - Last update timestamp

## Session Keys

### Stored in Session
- `UserName` - Displayed in navbar
- `UserEmail` - Internal reference

### Timeout
- **30 minutes** of inactivity
- Automatic logout after timeout
- Redirect to login page

## URLs Map

| Path | Method | Purpose |
|------|--------|---------|
| `/Login` | GET | Display login form |
| `/Login` | POST | Process login |
| `/Logout` | GET | Logout user |
| `/Admin/Dashboard` | GET | Admin home |
| `/Admin/Users` | GET | User list |
| `/Admin/Users/Create` | GET | Create form |
| `/Admin/Users/Create` | POST | Save user |
| `/Admin/Users/Edit/{id}` | GET | Edit form |
| `/Admin/Users/Edit/{id}` | POST | Update user |
| `/Admin/Users/Delete/{id}` | POST | Delete user |
| `/Admin/Users/Deactivate/{id}` | POST | Deactivate user |
| `/Admin/Users/Activate/{id}` | POST | Activate user |

## Key Files

### Models
- `Models/User.cs` - User entity

### Controllers
- `Controllers/LoginController.cs` - Authentication
- `Areas/Admin/Controllers/UsersController.cs` - User management

### Views
- `Areas/Admin/Views/Users/Index.cshtml` - User list
- `Areas/Admin/Views/Users/Create.cshtml` - Create form
- `Areas/Admin/Views/Users/Edit.cshtml` - Edit form

### Utilities
- `Utilities/PasswordHasher.cs` - Password hashing

### Database
- `Data/AppDbContext.cs` - Database context
- `Migrations/20260628213009_AddUserModel.cs` - Migration

## Error Messages (Turkish)

| Issue | Message |
|-------|---------|
| Empty fields | "alanı boş olamaz" (field cannot be empty) |
| Duplicate email | "Bu e-posta adresi zaten kullanılmaktadır" (Email already in use) |
| Weak password | "Şifre en az 8 karakter uzunluğunda olmalıdır" (Min 8 chars) |
| Password mismatch | "Şifreler eşleşmiyor" (Passwords don't match) |
| Invalid credentials | "E-posta veya şifre hatalı" (Email or password wrong) |
| User not found | "Kullanıcı bulunamadı" (User not found) |
| Last active user | "Son aktif kullanıcı silinemez" (Cannot delete last user) |

## Success Messages (Turkish)

| Action | Message |
|--------|---------|
| Create user | "'{name}' kullanıcısı başarıyla oluşturuldu" |
| Update user | "'{name}' kullanıcısı başarıyla güncellendi" |
| Delete user | "'{name}' kullanıcısı başarıyla silindi" |
| Deactivate | "'{name}' kullanıcısı devre dışı bırakıldı" |
| Activate | "'{name}' kullanıcısı etkinleştirildi" |

## Navigation

### Main Site Navbar
- Shows "Giriş Yap" (Login) if not logged in
- Shows user name dropdown if logged in
- Dropdown includes: Admin Panel, Logout

### Admin Panel Navbar
- Shows user name and logout option
- "Site Geri Dön" (Back to Site) link

### Admin Sidebar
- Dashboard
- Gallery (disabled)
- Services (disabled)
- Artists (enabled)
- **Users (NEW)**
- Testimonials (disabled)
- Messages (disabled)

## Security Notes

### Password Security
- All passwords hashed with BCrypt
- Never stored in plain text
- 11 salt rounds
- Passwords cannot be recovered (one-way hash)

### Session Security
- HttpOnly cookies (prevent XSS)
- SameSite=Lax (prevent CSRF)
- 30-minute timeout
- Cleared on logout

### Email Uniqueness
- Each user must have unique email
- Database enforces uniqueness constraint
- Duplicate emails rejected on create/edit

## Troubleshooting

### Can't Login
- Verify email is correct
- Verify password is correct
- Check if user is Active (not deactivated)
- Check if user exists in database

### Forgot Password
- Currently no password reset feature
- Admin must edit user and set new password
- User can then login with new password

### Session Expired
- 30-minute timeout of inactivity
- Click anything to extend session
- Auto-logout redirects to login page

### Can't Access Admin
- Must be logged in
- Must have active user account
- Check if user is marked Active in database

## Development URLs

| Service | URL |
|---------|-----|
| Web App | http://localhost:5000 |
| HTTPS | https://localhost:5001 |
| API | (Not applicable, MVC only) |
| Database | localhost:3306 (MySQL) |

## Build & Run

```bash
# Build
dotnet build

# Run
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update
```

## Database Backup

```bash
# Backup before deployment
mysqldump -u user -p database > backup.sql

# Restore if needed
mysql -u user -p database < backup.sql
```

---

**Last Updated**: June 28, 2026
**Version**: 1.0
**Status**: Production Ready
