using System.Diagnostics;
using inkfusion.MVC.Models;
using inkfusion.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inkfusion.MVC.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Load contact info for footer
                await LoadContactInfoAsync();

                // Fetch HeroSection (first active one)
                var heroSection = await _context.HeroSections
                    .Where(h => h.IsActive)
                    .FirstOrDefaultAsync();

                // Fetch all active Services ordered by Order
                var services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Order)
                    .ToListAsync();

                // Fetch all active Gallery items ordered by Order
                var galleries = await _context.Galleries
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.Order)
                    .ToListAsync();

                // Fetch all active Testimonials ordered by Rating descending
                var testimonials = await _context.Testimonials
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.Rating)
                    .ToListAsync();

                // Fetch all active ContactInfo items ordered by Key
                var contactInfo = await _context.ContactInfo
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Key)
                    .ToListAsync();

                // Fetch SiteSettings and convert to Dictionary
                var siteSettingsList = await _context.SiteSettings.ToListAsync();
                var siteSettings = siteSettingsList.ToDictionary(s => s.Key, s => s.Value);

                // Create and populate ViewModel
                var viewModel = new HomePageViewModel
                {
                    HeroSection = heroSection,
                    Services = services,
                    Galleries = galleries,
                    Testimonials = testimonials,
                    ContactInfo = contactInfo,
                    SiteSettings = siteSettings
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Home Index: {ex.Message}");
                return View(new HomePageViewModel());
            }
        }

        public async Task<IActionResult> Privacy()
        {
            // Load contact info for footer
            await LoadContactInfoAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            // Load contact info for footer
            await LoadContactInfoAsync();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
