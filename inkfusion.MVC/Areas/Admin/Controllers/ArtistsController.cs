using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inkfusion.MVC.Data;
using inkfusion.MVC.Models;

namespace inkfusion.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class ArtistsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ArtistsController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Artists
        public async Task<IActionResult> Index()
        {
            var artists = await _context.Artists.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return View(artists);
        }

        // GET: Admin/Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Artists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artist artist, IFormFile? imageFile)
        {
            try
            {
                // Handle image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    artist.ImageUrl = await SaveImageAsync(imageFile);
                }

                if (ModelState.IsValid)
                {
                    artist.CreatedAt = DateTime.UtcNow;
                    _context.Add(artist);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sanatçı başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }

            return View(artist);
        }

        // GET: Admin/Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Admin/Artists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artist artist, IFormFile? imageFile)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            Artist? existingArtist = null;
            try
            {
                existingArtist = await _context.Artists.FindAsync(id);
                if (existingArtist == null)
                {
                    return NotFound();
                }

                // Update properties
                existingArtist.Name = artist.Name;
                existingArtist.Specialty = artist.Specialty;
                existingArtist.Bio = artist.Bio;
                existingArtist.IsActive = artist.IsActive;

                // Handle image upload if new image provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingArtist.ImageUrl))
                    {
                        DeleteImageAsync(existingArtist.ImageUrl).Wait();
                    }

                    existingArtist.ImageUrl = await SaveImageAsync(imageFile);
                }

                if (ModelState.IsValid)
                {
                    _context.Update(existingArtist);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sanatçı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(artist.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }

            return View(existingArtist ?? artist);
        }

        // POST: Admin/Artists/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var artist = await _context.Artists.FindAsync(id);
                if (artist != null)
                {
                    // Delete image if exists
                    if (!string.IsNullOrEmpty(artist.ImageUrl))
                    {
                        await DeleteImageAsync(artist.ImageUrl);
                    }

                    _context.Artists.Remove(artist);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sanatçı başarıyla silindi.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to save image
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Desteklenmeyen dosya türü. Lütfen JPG, PNG, GIF veya WEBP yükleyin.");
            }

            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "img", "artists");

            // Create directory if it doesn't exist
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var filePath = Path.Combine(imagesPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/img/artists/{fileName}";
        }

        // Helper method to delete image
        private async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "img", "artists", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - we don't want to fail the operation because of image deletion
                Console.WriteLine($"Resim silme hatası: {ex.Message}");
            }
        }

        // Helper method to check if artist exists
        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
