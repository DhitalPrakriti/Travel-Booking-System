using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBookingSystem.Data;
using TravelBookingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBookingSystem.Controllers
{
    public class DestinationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<DestinationsController> _logger;

        public DestinationsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<DestinationsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Destinations
        public async Task<IActionResult> Index()
        {
            if (_context?.Destinations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Destinations' is null.");
            }

            return View(await _context.Destinations.ToListAsync());
        }

        // GET: Destinations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Destinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Destinations destination, IFormFile? ImagePath)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(destination.Location))
                {
                    ModelState.AddModelError("Location", "Location is required.");
                }

                if (ImagePath != null && ImagePath.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                    var extension = Path.GetExtension(ImagePath.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePath = Path.Combine(uploadsFolder, newFileName);

                    // Save the file to the uploads folder
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagePath.CopyToAsync(stream);
                    }

                    destination.ImagePath = $"/uploads/{newFileName}";
                }

                _context.Add(destination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(destination);
        }

        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("A valid destination ID must be provided.");
            }

            if (_context?.Destinations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Destinations' is null.");
            }

            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound($"Destination with ID {id} was not found.");
            }

            return View(destination);
        }

        // POST: Destinations/Edit/5
        // POST: Destinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Destinations destination, IFormFile? ImagePath)
        {
            if (id != destination.DestinationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImagePath != null && ImagePath.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                        var extension = Path.GetExtension(ImagePath.FileName);
                        var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        var filePath = Path.Combine(uploadsFolder, newFileName);

                        // Save the new image
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImagePath.CopyToAsync(stream);
                        }

                        destination.ImagePath = $"/uploads/{newFileName}";  // Update image path in destination model
                    }

                    _context.Update(destination);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destination.DestinationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(destination);
        }


        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context?.Destinations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Destinations' is null.");
            }

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            if (_context?.Destinations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Destinations' is null.");
            }

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.DestinationId == id);
            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Attempting to delete destination with ID: {DestinationId}", id);

            if (_context?.Destinations == null)
            {
                _logger.LogError("Destinations context is null.");
                return Problem("Entity set 'ApplicationDbContext.Destinations' is null.");
            }

            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                _logger.LogWarning("Destination with ID {DestinationId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Deleting destination with ID: {DestinationId}", id);
            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Destination deleted successfully.");

            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if the destination exists by ID
        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationId == id);
        }
    }
}
