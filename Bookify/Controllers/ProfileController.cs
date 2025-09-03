using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Edit()
        {
            return View();
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