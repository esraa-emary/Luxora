using Microsoft.AspNetCore.Mvc;
using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.Entities;
using Bookify.Attributes;
using Bookify.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly BookifyDbContext _context;

        public AdminController(BookifyDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            ViewBag.TotalRooms = await _context.Rooms.CountAsync();
            ViewBag.AvailableRooms = await _context.Rooms.CountAsync(r => r.Status == "Available");
            ViewBag.TotalReservations = await _context.Reservations.CountAsync();
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalRevenue = await _context.Reservations.SumAsync(r => (decimal?)r.Price) ?? 0;
            
            return View();
        }

        // Manage Rooms
        public async Task<IActionResult> ManageRooms()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
            
            return View(rooms);
        }

        // Add Room - GET
        public async Task<IActionResult> AddRoom()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
            return View();
        }

        // Add Room - POST
        [HttpPost]
        public async Task<IActionResult> AddRoom(int roomNumber, string description, string status, decimal price, int roomTypeId)
        {
            try
            {
                // Check if room number already exists
                var existingRoom = await _context.Rooms.FindAsync(roomNumber);
                if (existingRoom != null)
                {
                    ViewBag.Error = "Room number already exists.";
                    ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
                    return View();
                }

                var room = new Room
                {
                    RoomNumber = roomNumber,
                    Description = description,
                    Status = status,
                    Price = price,
                    RoomTypeId = roomTypeId
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Room added successfully!";
                return RedirectToAction("ManageRooms");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error adding room: " + ex.Message;
                ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
                return View();
            }
        }

        // Edit Room - GET
        public async Task<IActionResult> EditRoom(int id)
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomNumber == id);

            if (room == null)
            {
                return NotFound();
            }

            ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
            return View(room);
        }

        // Edit Room - POST
        [HttpPost]
        public async Task<IActionResult> EditRoom(int roomNumber, string description, string status, decimal price, int roomTypeId)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomNumber);
                if (room == null)
                {
                    return NotFound();
                }

                room.Description = description;
                room.Status = status;
                room.Price = price;
                room.RoomTypeId = roomTypeId;

                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Room updated successfully!";
                return RedirectToAction("ManageRooms");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating room: " + ex.Message;
                ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
                var room = await _context.Rooms.FindAsync(roomNumber);
                return View(room);
            }
        }

        // Delete Room
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found." });
                }

                // Check if room has reservations
                var hasReservations = await _context.Reservations.AnyAsync(r => r.RoomNumber == id);
                if (hasReservations)
                {
                    return Json(new { success = false, message = "Cannot delete room with existing reservations." });
                }

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Room deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting room: " + ex.Message });
            }
        }

        // View as User (switch to user interface)
        public IActionResult ViewAsUser()
        {
            return RedirectToAction("Index", "Home");
        }

        // Manage Reservations
        public async Task<IActionResult> ManageReservations()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .ThenInclude(room => room.RoomType)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();

            return View(reservations);
        }

        // Delete Reservation
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Room)
                    .FirstOrDefaultAsync(r => r.ReservationId == id);

                if (reservation == null)
                {
                    return Json(new { success = false, message = "Reservation not found." });
                }

                var roomNumber = reservation.RoomNumber;

                // Delete the reservation
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                // Check if room has any other active reservations
                var today = DateTime.Today;
                var hasOtherActiveReservations = await _context.Reservations
                    .AnyAsync(r => r.RoomNumber == roomNumber && 
                                  (r.Status == ReservationStatus.Pending || 
                                   (r.Status == ReservationStatus.Completed && r.EndDate >= today)));

                // If no other active reservations, make room available again
                if (!hasOtherActiveReservations)
                {
                    var room = await _context.Rooms.FindAsync(roomNumber);
                    if (room != null && room.Status == "Unavailable")
                    {
                        room.Status = "Available";
                        _context.Rooms.Update(room);
                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Reservation deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting reservation: " + ex.Message });
            }
        }

        // Manage Customers
        public async Task<IActionResult> ManageCustomers()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            var customers = await _context.Customers
                .Include(c => c.Reservations)
                .OrderByDescending(c => c.CustomerId)
                .ToListAsync();

            return View(customers);
        }
    }
}




