using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inkfusion.MVC.Data;
using inkfusion.MVC.Models;

namespace inkfusion.MVC.Controllers
{
    /// <summary>
    /// Base controller class that handles common operations for all controllers
    /// Automatically fetches and passes dynamic contact information to views
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger<BaseController> _logger;

        public BaseController(AppDbContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Fetches active contact information from the database and sets it in ViewBag
        /// This method should be called in actions that render views with the shared layout
        /// </summary>
        protected async Task LoadContactInfoAsync()
        {
            try
            {
                var contactInfo = await _context.ContactInfo
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Key)
                    .ToListAsync();

                ViewBag.ContactInfo = contactInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading contact info: {ex.Message}");
                ViewBag.ContactInfo = new List<ContactInfo>();
            }
        }
    }
}
