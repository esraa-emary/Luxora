using Bookify.Helpers;
using Microsoft.AspNetCore.Mvc;
using Bookify.Models;
namespace Bookify.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public IActionResult Payment()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new Payment();
            return View(model);
        }

        [HttpPost]
        public IActionResult Payment(Payment model)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "✅ Booking & Payment Successful!";
                return RedirectToAction("PaymentConfirmation");
            }

            return View(model);
        }

        public IActionResult PaymentConfirmation()
        {
            ViewBag.Message = TempData["SuccessMessage"];
            return View();
        }
    }
}
