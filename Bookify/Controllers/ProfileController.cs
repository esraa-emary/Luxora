using Bookify.DataAccessLayer;
using Bookify.Helpers;
using Bookify.Models;
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

        public async Task<IActionResult> Index()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

            if (customer == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var reservations = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId.Value)
                .OrderByDescending(r => r.ReservationDate)
                .ToList();

            ViewBag.Customer = customer;
            return View(reservations);
        }
        private int? GetCurrentCustomerId()
        {
            return SessionHelper.GetUserId(HttpContext.Session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = SessionHelper.GetUserId(HttpContext.Session);
            if (customerId == null)
            {
                ViewBag.Error = "User not found. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

            if (customer == null)
            {
                ViewBag.Error = "Customer not found.";
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new EditProfileViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.phoneNumber ?? string.Empty,
                Nationality = customer.nationality ?? string.Empty,
                Age = customer.Age,
                Address = customer.Address
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = SessionHelper.GetUserId(HttpContext.Session);
            if (customerId == null)
            {
                ViewBag.Error = "User not found. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

            if (customer == null)
            {
                ViewBag.Error = "Customer not found.";
                return RedirectToAction("Login", "Auth");
            }

            if (customer.Email != model.Email)
            {
                var emailExists = await _context.Customers
                    .AnyAsync(c => c.Email == model.Email && c.CustomerId != customerId.Value);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered to another account.");
                    return View(model);
                }
            }

            var oldEmail = customer.Email;
            var oldName = customer.Name;

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Name = model.Name;
            customer.Email = model.Email;
            customer.phoneNumber = model.PhoneNumber;
            customer.nationality = model.Nationality;
            customer.Age = model.Age;
            customer.Address = model.Address;

            try
            {
                if (oldEmail != customer.Email || oldName != customer.Name)
                {
                    var isAdmin = SessionHelper.IsAdmin(HttpContext.Session);
                    SessionHelper.SetUserSession(
                        HttpContext.Session,
                        customer.CustomerId,
                        customer.Name,
                        customer.Email,
                        isAdmin
                    );
                }

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ViewBag.Error = "An error occurred while updating your profile. Please try again.";
                return View(model);
            }
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