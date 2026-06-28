# Visual Reference Guide - Login System Flow

This document provides visual diagrams and flow charts for understanding the login system.

## 1. Authentication Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        LOGIN FLOW DIAGRAM                        │
└─────────────────────────────────────────────────────────────────┘

User visits /Login
      ↓
   ┌──────────────────┐
   │  Login Form      │
   │  Displayed       │
   └────────┬─────────┘
            ↓
    Enter Email & Password
            ↓
      ┌─────────────────────┐
      │ Click "Giriş Yap"   │
      │ (Login Button)      │
      └──────────┬──────────┘
                 ↓
      ┌──────────────────────────┐
      │ POST /Login              │
      │ (Send Credentials)       │
      └──────────┬───────────────┘
                 ↓
    ┌────────────────────────────────────┐
    │ Verify Credentials                 │
    │ (Constant-time comparison)         │
    └────────┬─────────────────────┬──────┘
             ↓                     ↓
        ✓ VALID              ✗ INVALID
             ↓                     ↓
    ┌─────────────────┐   ┌──────────────────┐
    │ Create Session  │   │ Show Error       │
    │ Set UserEmail   │   │ Stay on Login    │
    └────────┬────────┘   └──────────────────┘
             ↓
    ┌──────────────────────────────────┐
    │ Redirect to /Admin/Dashboard     │
    │ (302 Found)                      │
    └────────┬─────────────────────────┘
             ↓
    ┌──────────────────────────────────┐
    │ Dashboard Page Loaded            │
    │ - Check Session ✓                │
    │ - Display Welcome Message        │
    │ - Show User Email                │
    │ - Enable Logout                  │
    └──────────────────────────────────┘
```

## 2. Session Management Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      SESSION LIFECYCLE                           │
└─────────────────────────────────────────────────────────────────┘

    LOGIN SUCCESS
          ↓
    ┌─────────────────────────────────────┐
    │ Session Created                     │
    │ - ID: (auto-generated)              │
    │ - UserEmail: ramaza@example.com     │
    │ - CreatedAt: now                    │
    │ - LastActivity: now                 │
    │ - Timeout: 30 minutes               │
    └────────────┬────────────────────────┘
                 ↓
    ┌─────────────────────────────────────┐
    │ Session Cookie Set                  │
    │ - HttpOnly: true                    │
    │ - Secure: true (HTTPS)              │
    │ - SameSite: Lax                     │
    │ - Path: /                           │
    │ - Domain: localhost                 │
    └────────────┬────────────────────────┘
                 ↓
    ┌─────────────────────────────────────┐
    │ User Browsing Admin Pages           │
    │ - Each request sends session cookie │
    │ - LastActivity updated              │
    │ - Timeout timer resets              │
    └────────────┬────────────────────────┘
                 ↓
         ┌──────────┬──────────┐
         ↓                     ↓
    LOGOUT CLICKED      30 MIN INACTIVE
         ↓                     ↓
    ┌──────────────┐   ┌────────────────┐
    │ Clear Session│   │ Session Expires│
    │ Remove Cookie│   │ Auto Redirect  │
    │ Redirect Home│   │ to Login       │
    └──────────────┘   └────────────────┘
```

## 3. Authorization Check Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                   AUTHORIZATION CHECK FLOW                       │
└─────────────────────────────────────────────────────────────────┘

User Requests Protected Page
    (e.g., /Admin/Dashboard)
            ↓
    ┌──────────────────────────────────────┐
    │ [RequireLogin] Attribute Triggers    │
    │ (IAuthorizationFilter)               │
    └────────────┬─────────────────────────┘
                 ↓
    ┌──────────────────────────────────────┐
    │ Check Session for UserEmail          │
    │ session.GetString("UserEmail")       │
    └────────┬──────────────────┬──────────┘
             ↓                  ↓
        FOUND (✓)          NOT FOUND (✗)
             ↓                  ↓
    ┌─────────────────┐   ┌──────────────────────┐
    │ Allow Request   │   │ Redirect to /Login   │
    │ Continue to     │   │ (302 Found)          │
    │ Action Method   │   │ Remember Return URL  │
    └────────┬────────┘   └──────────────────────┘
             ↓
    ┌──────────────────────────────────────┐
    │ Execute Controller Action            │
    │ Return View                          │
    │ Display Protected Content            │
    └──────────────────────────────────────┘
```

## 4. Page State Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    NAVBAR STATE CHANGES                          │
└─────────────────────────────────────────────────────────────────┘

NOT LOGGED IN                      LOGGED IN
────────────────────────────────────────────

    ┌──────────────┐             ┌──────────────────┐
    │ Navbar       │             │ Navbar           │
    ├──────────────┤             ├──────────────────┤
    │ Home         │             │ Home             │
    │ About        │             │ About            │
    │ Services     │             │ Services         │
    │ Gallery      │             │ Gallery          │
    │ Artists      │             │ Artists          │
    │ Testimonials │             │ Testimonials     │
    │ Contact      │             │ Contact          │
    │ ┌──────────┐ │             │ ┌──────────────┐ │
    │ │Giriş Yap │ │ ←→ after   │ │▼ user@ex.com │ │
    │ │(login)   │ │ clicking   │ │ └─────────┬──┘ │
    │ └──────────┘ │ logout     │ │  ├Dashboard│   │
    │              │            │ │  ├Artists  │   │
    │              │            │ │  └Logout   │   │
    └──────────────┘            └──────────────────┘
```

## 5. Database/Storage Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│              SESSION STORAGE (In-Memory)                         │
└─────────────────────────────────────────────────────────────────┘

Application Memory
┌──────────────────────────────────────────────┐
│ Session Store (IDistributedCache equivalent) │
├──────────────────────────────────────────────┤
│                                              │
│ Key: {SessionId1}                           │
│ Value: {                                     │
│   "UserEmail": "ramaza@example.com",        │
│   "LastActivity": "2026-06-29T14:30:00Z",  │
│   "CreatedAt": "2026-06-29T14:00:00Z"      │
│ }                                            │
│                                              │
│ Key: {SessionId2}                           │
│ Value: {                                     │
│   "UserEmail": "admin@example.com",         │
│   "LastActivity": "2026-06-29T15:15:00Z",  │
│   "CreatedAt": "2026-06-29T15:00:00Z"      │
│ }                                            │
│                                              │
└──────────────────────────────────────────────┘

Browser Cookie
┌────────────────────────────────┐
│ Session Cookie                 │
├────────────────────────────────┤
│ Name: .AspNetCore.Session      │
│ Value: [encrypted session ID]  │
│ HttpOnly: true                 │
│ Secure: true                   │
│ SameSite: Lax                  │
│ Path: /                        │
│ Expires: [session timeout]     │
└────────────────────────────────┘
```

## 6. Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                  COMPONENT INTERACTIONS                          │
└─────────────────────────────────────────────────────────────────┘

                        ┌──────────────┐
                        │   Browser    │
                        │   (User)     │
                        └──────┬───────┘
                               │
                               ↓
                        ┌──────────────────┐
                        │  Web Server      │
                        │  ASP.NET Core    │
                        └────────┬─────────┘
                                 │
          ┌──────────────────────┼──────────────────────┐
          ↓                      ↓                      ↓
    ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
    │LoginController│     │[RequireLogin]│     │Session Store │
    │              │     │  Attribute   │     │(In-Memory)   │
    │- GET /Login  │     │              │     │              │
    │- POST /Login │     │- OnAuthorize │     │- Create      │
    │- GET /Logout │     │  (Intercept) │     │- Get         │
    │              │     │- Check Login │     │- Clear       │
    │- Verify Creds│     │- Redirect    │     │- Timeout     │
    │- Create Sess │     │  to Login    │     │              │
    │- Clear Sess  │     └──────────────┘     └──────────────┘
    └──────────────┘
         │
         ├─ Views/Login/Login.cshtml
         │  └─ HTML + Bootstrap + Dark Theme
         │
         └─ _Layout.cshtml
            └─ Navbar with conditional login/dropdown
```

## 7. User Journey Map

```
┌─────────────────────────────────────────────────────────────────┐
│                      USER JOURNEY MAP                            │
└─────────────────────────────────────────────────────────────────┘

SCENARIO: First-Time Admin Login

START (Unauthenticated)
  │
  ├─ 1. Clicks "Giriş Yap" button on navbar
  │        ↓
  ├─ 2. Redirected to /Login
  │        ↓
  ├─ 3. Enters email: ramaza@example.com
  │        ↓
  ├─ 4. Enters password: Ramazan2026.R
  │        ↓
  ├─ 5. Clicks "Giriş Yap" button
  │    Loading spinner appears
  │        ↓
  ├─ 6. Server verifies credentials ✓
  │        ↓
  ├─ 7. Session created with UserEmail
  │        ↓
  ├─ 8. Redirected to /Admin/Dashboard
  │        ↓
  ├─ 9. Dashboard displays:
  │    - Welcome message
  │    - Navbar shows user dropdown
  │    - Admin stats and features available
  │        ↓
  ├─ 10. User navigates through admin pages
  │    - Artists management
  │    - Dashboard features
  │    - All pages protected by [RequireLogin]
  │        ↓
  ├─ 11. User clicks dropdown → "Çıkış Yap"
  │        ↓
  ├─ 12. Session cleared
  │        ↓
  ├─ 13. Redirected to Home page
  │        ↓
  └─ 14. Navbar shows "Giriş Yap" again
         (Back to start)
```

## 8. Security Layers Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    SECURITY LAYERS                               │
└─────────────────────────────────────────────────────────────────┘

REQUEST
  ↓
┌─────────────────────────────────────────┐
│ Layer 1: HTTPS Transport                │
│ - Encryption in transit                 │
│ - Certificate validation                │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│ Layer 2: CSRF Protection                │
│ - Anti-forgery token validation         │
│ - Token in form                         │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│ Layer 3: Credential Verification        │
│ - Constant-time comparison              │
│ - Email + Password match                │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│ Layer 4: Session Management             │
│ - HttpOnly cookie                       │
│ - Secure flag for HTTPS                 │
│ - SameSite=Lax for CSRF                 │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│ Layer 5: Authorization Check            │
│ - [RequireLogin] attribute              │
│ - Session validation                    │
│ - User identity verification            │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│ Layer 6: Access Control                 │
│ - Protected controller actions          │
│ - Resource-level permissions            │
└──────────────┬──────────────────────────┘
               ↓
          RESPONSE
```

## 9. Error Handling Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                  ERROR HANDLING FLOW                             │
└─────────────────────────────────────────────────────────────────┘

Login Attempt
  ↓
┌──────────────────────────┐
│ Input Validation         │
└─────┬──────────────┬──────┘
      ↓              ↓
   VALID          INVALID
      ↓              ↓
      │         ┌─────────────┐
      │         │ Return Form │
      │         │ + Message:  │
      │         │"Email & pwd │
      │         │ required"   │
      │         └─────────────┘
      ↓
┌──────────────────────┐
│ Credential Check     │
└─────┬──────────────┬─┘
      ↓              ↓
   MATCH         NO MATCH
      ↓              ↓
      │         ┌──────────────┐
      │         │ Return Form  │
      │         │ + Message:   │
      │         │"Email or pwd │
      │         │incorrect"    │
      │         └──────────────┘
      │         LOG: Failed attempt
      ↓
┌──────────────────────┐
│ Create Session       │
│ & Redirect           │
└──────────────────────┘
```

## 10. Timeline Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                   SESSION TIMELINE                               │
└─────────────────────────────────────────────────────────────────┘

TIME             EVENT                         STATUS
────────────────────────────────────────────────────────
14:00:00    ✓ Login Success                 Created
14:00:01    Set Session UserEmail           Active
14:00:02    Redirect to Dashboard           Active
14:05:00    User browsing Dashboard         Active (refreshed)
14:10:00    User viewing Artists            Active (refreshed)
14:20:00    User idle, no requests          Active
14:30:00    ⚠ Session Timeout Limit        
14:30:01    Next request to protected page  
            → Check session expired         Expired
            → Redirect to /Login            
14:30:02    User back on login page         Expired
────────────────────────────────────────────────────────

OR

14:30:00    User clicks Logout               
14:30:01    ✓ Logout Confirmed             Cleared
14:30:02    Session removed                 Deleted
14:30:03    Redirect to Home                Logged Out
```

## 11. File Structure Tree

```
inkfusion.MVC/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── InkfusionController.cs
│   └── LoginController.cs ........................... NEW
│
├── Attributes/
│   └── RequireLoginAttribute.cs ..................... NEW
│
├── Views/
│   ├── Home/
│   ├── Inkfusion/
│   ├── Login/ ...................................... NEW DIRECTORY
│   │   └── Login.cshtml ............................ NEW
│   └── Shared/
│       ├── _Layout.cshtml ......................... MODIFIED
│       ├── _ViewImports.cshtml
│       └── _ViewStart.cshtml
│
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       │   ├── DashboardController.cs ............ MODIFIED
│       │   └── ArtistsController.cs ............. MODIFIED
│       └── Views/
│           └── Dashboard/
│               └── Index.cshtml ................. MODIFIED
│
├── Models/
├── Data/
└── Program.cs .................................... MODIFIED
```

## 12. Request/Response Timeline

```
┌─────────────────────────────────────────────────────────────────┐
│               COMPLETE LOGIN REQUEST/RESPONSE                    │
└─────────────────────────────────────────────────────────────────┘

CLIENT                              SERVER
  │                                   │
  ├─ GET /Login ──────────────────────→
  │                                   │
  │  ← 200 OK (HTML form) ────────────┤
  │  + Set-Cookie: (session id)       │
  │  + CSRF Token in form             │
  │                                   │
  │  [User fills form]                │
  │                                   │
  ├─ POST /Login ─────────────────────→
  │  + email=ramaza@example.com       │
  │  + password=Ramazan2026.R         │
  │  + __RequestVerificationToken     │
  │  + Cookie: (session id)           │
  │                                   │
  │                                   ├─ Verify CSRF token ✓
  │                                   ├─ Validate input ✓
  │                                   ├─ Compare credentials ✓
  │                                   ├─ Create session ✓
  │                                   ├─ Log event ✓
  │                                   │
  │  ← 302 Found ──────────────────────┤
  │  Location: /Admin/Dashboard       │
  │  Set-Cookie: (session updated)    │
  │                                   │
  ├─ GET /Admin/Dashboard ────────────→
  │  + Cookie: (session id)           │
  │                                   │
  │                                   ├─ [RequireLogin] checks
  │                                   ├─ Verify session exists ✓
  │                                   ├─ Get UserEmail ✓
  │                                   │
  │  ← 200 OK (Dashboard HTML) ────────┤
  │  + User info                      │
  │  + Dashboard content              │
  │                                   │
```

---

This visual guide provides a clear understanding of how the login system works from multiple perspectives: flow, user journey, security layers, and technical implementation.

For detailed information, refer to the comprehensive documentation files in the project root.
