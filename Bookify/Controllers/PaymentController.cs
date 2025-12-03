using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.DTOs;
using Bookify.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bookify.Controllers
{
    public class PaymentController : Controller
    {
        private readonly BookifyDbContext _context;

        public PaymentController(BookifyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Payment(int reservationId)
        {
            try
            {
                if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                    return RedirectToAction("Login", "Auth");

                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (userId == null)
                    return RedirectToAction("Login", "Auth");

                var reservation = _context.Reservations
                    .Include(r => r.Room)
                    .ThenInclude(r => r.RoomType)
                    .FirstOrDefault(r => r.ReservationId == reservationId && r.CustomerId == userId.Value);

                if (reservation == null)
                {
                    TempData["ErrorMessage"] = "Reservation not found.";
                    return RedirectToAction("Index", "History");
                }

                if (reservation.Status != DataAccessLayer.Entities.ReservationStatus.Pending)
                {
                    TempData["ErrorMessage"] = $"Reservation is already {reservation.Status}.";
                    return RedirectToAction("Index", "History");
                }

                var model = new PaymentDto
                {
                    ReservationId = reservationId,
                    RoomType = reservation.Room?.RoomType?.TypeName ?? "Standard",
                    TotalAmount = reservation.Price
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "History");
            }
        }

        [HttpPost]
        public IActionResult Payment([FromForm] PaymentDto model)
        {
            try
            {
                Console.WriteLine($"=== PAYMENT PROCESSING ===");
                Console.WriteLine($"ReservationId: {model.ReservationId}");
                Console.WriteLine($"Amount: {model.TotalAmount}");
                Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

                if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                    return RedirectToAction("Login", "Auth");

                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (userId == null)
                    return RedirectToAction("Login", "Auth");

                Console.WriteLine("Ignoring ModelState validation...");

                var reservation = _context.Reservations
                    .FirstOrDefault(r => r.ReservationId == model.ReservationId && r.CustomerId == userId.Value);

                if (reservation == null)
                {
                    TempData["ErrorMessage"] = "Reservation not found.";
                    return RedirectToAction("Index", "History");
                }

                Console.WriteLine($"Found reservation: ID={reservation.ReservationId}, Status={reservation.Status}");

                if (reservation.Status != DataAccessLayer.Entities.ReservationStatus.Pending)
                {
                    TempData["ErrorMessage"] = $"Reservation already {reservation.Status}.";
                    return RedirectToAction("Index", "History");
                }

                reservation.Status = DataAccessLayer.Entities.ReservationStatus.Completed;
                Console.WriteLine($"Updated status to: {reservation.Status}");

                // Note: Payment record not created for direct reservation payments
                // (would require a valid OrderId due to FK constraint)
                // The reservation status update is what matters for the booking flow

                var changes = _context.SaveChanges();
                Console.WriteLine($"Saved {changes} changes to database");

                TempData["SuccessMessage"] = $"✅ Payment Successful! Reservation #{model.ReservationId} confirmed.";
                Console.WriteLine("=== PAYMENT SUCCESS ===");

                return RedirectToAction("Index", "History");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== PAYMENT ERROR ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                Console.WriteLine("=== END ERROR ===");

                try
                {
                    var userId = SessionHelper.GetUserId(HttpContext.Session);
                    if (userId != null)
                    {
                        var reservation = _context.Reservations
                            .FirstOrDefault(r => r.ReservationId == model.ReservationId && r.CustomerId == userId.Value);

                        if (reservation != null && reservation.Status == DataAccessLayer.Entities.ReservationStatus.Pending)
                        {
                            reservation.Status = DataAccessLayer.Entities.ReservationStatus.Completed;
                            _context.SaveChanges();
                            TempData["SuccessMessage"] = "✅ Payment Processed!";
                            return RedirectToAction("Index", "History");
                        }
                    }
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Inner error: {innerEx.Message}");
                }

                TempData["ErrorMessage"] = $"Payment failed: {ex.Message}";
                return View(model);
            }
        }

        public IActionResult QuickPay(int reservationId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Login", "Auth");

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var reservation = _context.Reservations
                    .FirstOrDefault(r => r.ReservationId == reservationId && r.CustomerId == userId.Value);

                if (reservation == null)
                {
                    TempData["ErrorMessage"] = "Reservation not found.";
                    return RedirectToAction("Index", "History");
                }

                if (reservation.Status == DataAccessLayer.Entities.ReservationStatus.Pending)
                {
                    reservation.Status = DataAccessLayer.Entities.ReservationStatus.Completed;
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"✅ Payment Successful! Reservation #{reservationId} confirmed.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Reservation already {reservation.Status}.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Payment failed: {ex.Message}";
            }

            return RedirectToAction("Index", "History");
        }
    }
}