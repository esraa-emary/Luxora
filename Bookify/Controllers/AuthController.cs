using Microsoft.AspNetCore.Mvc;
using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.Entities;
using Bookify.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class AuthController : Controller
    {
        private readonly BookifyDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BookifyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            // If already logged in, redirect appropriately
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                if (SessionHelper.IsAdmin(HttpContext.Session))
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Signup()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null || !PasswordHasher.VerifyPassword(password, customer.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Check if user is admin
            var adminEmails = _configuration.GetSection("AdminEmails").Get<List<string>>() ?? new List<string>();
            bool isAdmin = adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);

            // Set session
            SessionHelper.SetUserSession(
                HttpContext.Session,
                customer.CustomerId,
                customer.Name,
                customer.Email,
                isAdmin
            );

            // Redirect based on role
            if (isAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string firstName, string lastName, string name, string ssn, int age, string address, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            // Check if email already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (existingCustomer != null)
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            // Check if SSN already exists
            var existingSSN = await _context.Customers
                .FirstOrDefaultAsync(c => c.SSN == ssn);

            if (existingSSN != null)
            {
                ViewBag.Error = "SSN already registered.";
                return View();
            }

            // Create new customer
            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Name = name,
                SSN = ssn,
                Age = age,
                Address = address,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Auto login after signup
            var adminEmails = _configuration.GetSection("AdminEmails").Get<List<string>>() ?? new List<string>();
            bool isAdmin = adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);

            SessionHelper.SetUserSession(
                HttpContext.Session,
                customer.CustomerId,
                customer.Name,
                customer.Email,
                isAdmin
            );

            if (isAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            SessionHelper.ClearSession(HttpContext.Session);
            return RedirectToAction("Index", "Home");
        }
    }
}

