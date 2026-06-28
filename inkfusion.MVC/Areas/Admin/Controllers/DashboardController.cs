using Microsoft.AspNetCore.Mvc;
using inkfusion.MVC.Attributes;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [RequireLogin]
    public class DashboardController : Controller
    {
        private const string USER_SESSION_KEY = "UserName";
        private const string USER_EMAIL_SESSION_KEY = "UserEmail";

        public IActionResult Index()
        {
            // Pass logged-in user name to view
            var userName = HttpContext.Session.GetString(USER_SESSION_KEY);
            var userEmail = HttpContext.Session.GetString(USER_EMAIL_SESSION_KEY);

            ViewBag.CurrentUser = userName;
            ViewBag.CurrentUserEmail = userEmail;

            return View();
        }
    }
}
