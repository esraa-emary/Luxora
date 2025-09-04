using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult MakeOrder()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

    }
}
