using Microsoft.AspNetCore.Mvc;
using TravelBookingSystem.Data;
using TravelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using TravelBookingSystem.Services;

namespace TravelBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public BookingsController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));  // Ensure _context is not null
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));  // Ensure _emailService is not null
        }

        // GET: Bookings/Index
        public async Task<IActionResult> Index()
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bookings' is null.");
            }

            // Fetch bookings with associated destinations
            var bookings = await _context.Bookings.Include(b => b.Destination).ToListAsync();

            Console.WriteLine($"Bookings found: {bookings.Count}");
            foreach (var booking in bookings)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}, User: {booking.UserName}, Destination: {booking.Destination?.Name ?? "No Destination"}");
            }
            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            // Populate the dropdown with available destinations for the user to select
            ViewBag.Destinations = _context.Destinations.ToList();
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,DestinationId,BookingDate")] Bookings bookings)
        {
            if (ModelState.IsValid)
            {
                // Ensure UserName is provided
                if (string.IsNullOrEmpty(bookings.UserName))
                {
                    ModelState.AddModelError("UserName", "Email address is required.");
                    return View(bookings);
                }

                // Ensure DestinationId is valid
                var destination = await _context.Destinations.FindAsync(bookings.DestinationId);
                if (destination == null)
                {
                    ModelState.AddModelError("DestinationId", "Invalid destination selected.");
                    return View(bookings);
                }

                // Add booking to the database
                try
                {
                    bookings.BookingDate = DateTime.Now;  // Set the booking date to the current time
                    _context.Bookings.Add(bookings);  // Add the new booking
                    await _context.SaveChangesAsync();  // Save the changes to the database
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing your booking: " + ex.Message);
                    return View(bookings);
                }

                // Send confirmation email
                await _emailService.SendEmailAsync(
                    bookings.UserName,
                    "Booking Confirmation",
                    $"Your booking for {destination.Name} has been confirmed!"
                );

                return RedirectToAction(nameof(Index));  // Redirect to the bookings list page
            }

            // Repopulate the destination list in case of validation failure
            ViewBag.Destinations = _context.Destinations.ToList();
            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("The booking ID was not provided.");
            }

            var booking = await _context.Bookings.Include(b => b.Destination)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} was not found.");
            }

            return View(booking);
        }
        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound("Booking ID is required.");
            }

            var booking = await _context.Bookings.Include(b => b.Destination)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return View(booking);  // Pass the booking model to the view
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  // Redirect back to the bookings list
        }

    }
}
