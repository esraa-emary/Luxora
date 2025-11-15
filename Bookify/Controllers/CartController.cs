using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class CartController : Controller
    {
        private readonly BookifyDbContext _context;

        public CartController(BookifyDbContext context)
        {
            _context = context;
        }

        // Fake UserId = 1 for testing
        private int CurrentUser => 1;

        public async Task<IActionResult> Index()
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Room)
                .Where(c => c.UserId == CurrentUser)
                .ToListAsync();

            ViewBag.CartCount = cartItems.Count;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int roomNumber)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found" });
                }

                // Check if already in cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.RoomNumber == roomNumber && c.UserId == CurrentUser);

                if (existingItem != null)
                {
                    return Json(new { success = false, message = "Room already in cart" });
                }

                var cartItem = new CartItem
                {
                    RoomNumber = roomNumber,
                    UserId = CurrentUser
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Room added to cart" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding to cart" });
            }
        }

        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            var items = _context.CartItems.Where(c => c.UserId == CurrentUser);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetCartItems()
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == CurrentUser)
                    .Select(c => new
                    {
                        roomNumber = c.RoomNumber
                    })
                    .ToListAsync();

                return Json(cartItems);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> IsRoomInCart(int roomNumber)
        {
            var isInCart = await _context.CartItems
                .AnyAsync(c => c.RoomNumber == roomNumber && c.UserId == CurrentUser);

            return Json(new { isInCart = isInCart });
        }
    }
}