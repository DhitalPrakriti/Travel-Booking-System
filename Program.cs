using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using TravelBookingSystem.Data;
using TravelBookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
// Ensure your database connection string is correct in appsettings.json or environment variables
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TravelBookingDb")));

builder.Services.AddLogging();
//Register EmailService here
builder.Services.AddScoped<EmailService>(); // Ensure your EmailService is registered

// Add MVC services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Error handling in production
    app.UseHsts();  // HTTP Strict Transport Security (HSTS)
}
else
{
    // Use Developer Exception Page in development for detailed error info
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Serve static files (like images, CSS, JS)
app.UseStaticFiles();

// Default route configuration for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
