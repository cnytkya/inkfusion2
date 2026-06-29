using Microsoft.AspNetCore.Mvc;
using inkfusion.MVC.Data;

namespace inkfusion.MVC.Controllers
{
    public class InkfusionController : BaseController
    {
        public InkfusionController(ILogger<InkfusionController> logger, AppDbContext context)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> gallery()
        {
            // Load contact info for footer
            await LoadContactInfoAsync();
            return View();
        }
    }
}
