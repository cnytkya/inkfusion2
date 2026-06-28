using Microsoft.AspNetCore.Mvc;
using inkfusion.MVC.Attributes;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [RequireLogin]
    public class DashboardController : Controller
    {
        private const string USER_SESSION_KEY = "UserEmail";

        public IActionResult Index()
        {
            // Pass logged-in user email to view
            var userEmail = HttpContext.Session.GetString(USER_SESSION_KEY);
            ViewBag.CurrentUser = userEmail;

            return View();
        }
    }
}
