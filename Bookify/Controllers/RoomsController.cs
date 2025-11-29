using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.DataAccessLayer.Entities;
using Bookify.DataAccessLayer;

namespace Bookify.Controllers
{
    public class RoomsController : Controller
    {
        private readonly BookifyDbContext _context;

        public RoomsController(BookifyDbContext context)
        {
            _context = context;
        }

        // Returns the main view with rooms list
        public async Task<IActionResult> Index()
        {
            // Get all rooms and check for active reservations
            var allRooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                .ToListAsync();

            // Update room availability based on reservations
            var today = DateTime.Today;
            foreach (var room in allRooms)
            {
                var hasActiveReservation = room.Reservations.Any(r => 
                    r.Status == ReservationStatus.Pending || 
                    (r.Status == ReservationStatus.Completed && r.EndDate >= today));

                // If room has active reservation, mark as unavailable
                if (hasActiveReservation)
                {
                    if (room.Status != "Unavailable")
                    {
                        room.Status = "Unavailable";
                        _context.Rooms.Update(room);
                    }
                }
                // If no active reservations and room is unavailable, make it available again
                else if (room.Status != null && room.Status.Equals("Unavailable", StringComparison.OrdinalIgnoreCase))
                {
                    room.Status = "Available";
                    _context.Rooms.Update(room);
                }
            }
            await _context.SaveChangesAsync();

            // Return only available rooms
            var rooms = allRooms
                .Where(r => r.Status != null && r.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View(rooms);
        }

        // API endpoint: returns JSON for front-end
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            // Get all rooms and check for active reservations
            var allRooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                .ToListAsync();

            // Update room availability based on reservations
            var today = DateTime.Today;
            foreach (var room in allRooms)
            {
                var hasActiveReservation = room.Reservations.Any(r => 
                    r.Status == ReservationStatus.Pending || 
                    (r.Status == ReservationStatus.Completed && r.EndDate >= today));

                // If room has active reservation, mark as unavailable
                if (hasActiveReservation)
                {
                    if (room.Status != "Unavailable")
                    {
                        room.Status = "Unavailable";
                        _context.Rooms.Update(room);
                    }
                }
                // If no active reservations and room is unavailable, make it available again
                else if (room.Status != null && room.Status.Equals("Unavailable", StringComparison.OrdinalIgnoreCase))
                {
                    room.Status = "Available";
                    _context.Rooms.Update(room);
                }
            }
            await _context.SaveChangesAsync();

            // Return only available rooms
            var rooms = allRooms
                .Where(r => r.Status != null && r.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                .Select(r => new
                {
                    //Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomName = r.RoomName,
                    Price = r.Price,
                    Status = r.Status,
                    Capacity = r.Capacity,
                    Size = r.Size,
                    BedType = r.BedType,
                    View = r.View,
                    Amenities = r.Amenities,
                    ImageUrl = r.ImageUrl,
                    Description = r.Description,
                    Type = r.RoomType.TypeName
                    //Featured = r.Featured,
                    //Premium = r.Premium
                })
                .ToList();

            return Ok(rooms);
        }
    }
}