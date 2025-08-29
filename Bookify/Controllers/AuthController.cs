using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
      
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Signup(string firstName, string lastName, string email, string password, string confirmPassword)
        {
          
            return RedirectToAction("Index", "Home");
        }
    }
}

