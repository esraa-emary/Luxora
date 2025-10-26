using Microsoft.AspNetCore.Mvc;
using Bookify.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class RoomsController : Controller
    {
        private readonly BookifyDbContext _context;

        public RoomsController(BookifyDbContext context)
        {
            _context = context;
        }

        public IActionResult MakeOrder()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.Status == "Available")
                .OrderBy(r => r.Price)
                .ToListAsync();

            return View(rooms);
        }
    }
}
