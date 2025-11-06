using Bookify.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BookifyDbContext _context;

        public ProfileController(BookifyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get current user's reservations - you'll need to implement GetCurrentUserId()
            var reservations = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == GetCurrentCustomerId())
                .OrderByDescending(r => r.ReservationDate)
                .ToList();

            return View(reservations);
        }
        private int? GetCurrentCustomerId()
        {
            // If you're using ASP.NET Core Identity, use:
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // For session-based authentication:
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // For demo purposes, you can use a temporary approach:
            if (customerId == null)
            {
                // Try to get the first customer as fallback (remove this in production)
                var firstCustomer = _context.Customers.FirstOrDefault();
                if (firstCustomer != null)
                {
                    // Store in session for subsequent requests
                    HttpContext.Session.SetInt32("CustomerId", firstCustomer.CustomerId);
                    return firstCustomer.CustomerId;
                }
            }

            return customerId;
        }

        public IActionResult Edit()
        {
            return View();
        }
        
        public IActionResult ChangePassword()
        {
            return View();
        }
        
        public IActionResult Reviews()
        {
            return View();
        }
        
        public IActionResult Logout()
        {
            return View();
        }
    }
}