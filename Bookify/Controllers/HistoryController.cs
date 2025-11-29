using Bookify.DataAccessLayer;
using Bookify.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class HistoryController : Controller
    {
        private readonly BookifyDbContext _context;
        
        public HistoryController(BookifyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = SessionHelper.GetUserId(HttpContext.Session);
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only show reservations for the logged-in user
            var reservations = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId.Value)
                .OrderByDescending(r => r.ReservationDate)
                .ToList();
                
            return View(reservations);
        }
    }
}