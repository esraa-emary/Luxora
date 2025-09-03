using Bookify.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Bookify.DataAccessLayer;
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
            var reservation = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .ToList();
            return View(reservation);
        }
    }
}