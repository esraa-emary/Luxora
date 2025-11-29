using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.Entities;
using Bookify.Helpers;
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

        private int? GetCurrentUserId()
        {
            return SessionHelper.GetUserId(HttpContext.Session);
        }

        public async Task<IActionResult> Index()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Room)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            ViewBag.CartCount = cartItems.Count;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int roomNumber)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return Json(new { success = false, message = "Please log in to add items to cart" });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }

            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found" });
                }

                // Check if room is available
                if (room.Status != null && room.Status.Equals("Unavailable", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "This room is currently unavailable" });
                }

                // Check if already in cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.RoomNumber == roomNumber && c.UserId == userId.Value);

                if (existingItem != null)
                {
                    return Json(new { success = false, message = "Room already in cart" });
                }

                var cartItem = new CartItem
                {
                    RoomNumber = roomNumber,
                    UserId = userId.Value
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
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                // Security check: Ensure user can only remove their own cart items
                if (item.UserId != userId.Value)
                {
                    TempData["Error"] = "You can only remove your own cart items.";
                    return RedirectToAction("Index");
                }

                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var items = _context.CartItems.Where(c => c.UserId == userId.Value);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetCartItems()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return Json(new List<object>());
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new List<object>());
            }

            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId.Value)
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
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return Json(new { isInCart = false });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { isInCart = false });
            }

            var isInCart = await _context.CartItems
                .AnyAsync(c => c.RoomNumber == roomNumber && c.UserId == userId.Value);

            return Json(new { isInCart = isInCart });
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Get all cart items
                var cartItems = await _context.CartItems
                    .Include(c => c.Room)
                    .Where(c => c.UserId == userId.Value)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index");
                }

                // Set default dates: tomorrow as check-in, day after as check-out
                var checkInDate = DateTime.Today.AddDays(1);
                var checkOutDate = checkInDate.AddDays(1);
                var reservationDate = DateTime.Now;

                // Create a reservation for each cart item and mark room as unavailable
                var reservationsCreated = 0;
                var today = DateTime.Today;
                
                foreach (var cartItem in cartItems)
                {
                    // Check if room already has an active reservation
                    var existingReservation = await _context.Reservations
                        .AnyAsync(r => r.RoomNumber == cartItem.RoomNumber && 
                                      (r.Status == ReservationStatus.Pending || 
                                       (r.Status == ReservationStatus.Completed && r.EndDate >= today)));

                    if (existingReservation)
                    {
                        // Skip this room - it's already reserved
                        continue;
                    }

                    // Check if room is already marked as unavailable
                    if (cartItem.Room.Status != null && cartItem.Room.Status.Equals("Unavailable", StringComparison.OrdinalIgnoreCase))
                    {
                        // Skip this room - it's unavailable
                        continue;
                    }

                    // Update room status to Unavailable
                    cartItem.Room.Status = "Unavailable";
                    _context.Rooms.Update(cartItem.Room);

                    var reservation = new Reservation
                    {
                        CustomerId = userId.Value, // UserId is the CustomerId
                        RoomNumber = cartItem.RoomNumber,
                        StartDate = checkInDate,
                        EndDate = checkOutDate,
                        ReservationDate = reservationDate,
                        Price = cartItem.Room.Price,
                        Status = ReservationStatus.Pending
                    };

                    _context.Reservations.Add(reservation);
                    reservationsCreated++;
                }

                if (reservationsCreated == 0)
                {
                    TempData["Error"] = "All rooms in your cart are already reserved or unavailable.";
                    return RedirectToAction("Index");
                }

                // Clear the cart
                _context.CartItems.RemoveRange(cartItems);

                // Save all changes
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Successfully created {reservationsCreated} reservation(s)!";
                return RedirectToAction("Index", "History");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while processing your reservation. Please try again.";
                return RedirectToAction("Index");
            }
        }
    }
}