using Microsoft.AspNetCore.Mvc;
using Bookify.Models;
using Bookify;
namespace Bookify.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public IActionResult Payment()
        {
            var model = new Payment();
            return View(model);
        }

        [HttpPost]
        public IActionResult Payment(Payment model)
        {
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
