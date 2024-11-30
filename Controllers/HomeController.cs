using Microsoft.AspNetCore.Mvc; // For Controller, IActionResult, HttpGet, HttpPost, etc.
using Microsoft.Extensions.Configuration;  // For IConfiguration
using TravelBookingSystem.Services;  // For EmailService
using TravelBookingSystem.Models;
using System.Diagnostics;

namespace TravelBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        // Constructor to inject services
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, EmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;

            // Log the email configuration values for debugging purposes
            _logger.LogInformation("Sender Email: " + _configuration["EmailSettings:SenderEmail"]);
            _logger.LogInformation("SMTP Server: " + _configuration["EmailSettings:SmtpServer"]);
            _logger.LogInformation("Port: " + _configuration["EmailSettings:Port"]);
            _logger.LogInformation("Use SSL: " + _configuration["EmailSettings:UseSsl"]);
            _logger.LogInformation("Password is set: " + (_configuration["EmailSettings:Password"] != null ? "Yes" : "No"));

        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/SendTestEmail
        [HttpGet]
        public IActionResult SendTestEmail()
        {
            return View();
        }

        // POST: /Home/SendTestEmail
        [HttpPost]
        public async Task<IActionResult> SendTestEmail(string toEmail, string subject, string body)
        {
            try
            {
                await _emailService.SendEmailAsync(toEmail, subject, body);
                ViewData["EmailStatus"] = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                ViewData["Error"] = $"Error sending email: {ex.Message}";
            }

            return View();
        }

        // Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}