using Bookify.DataAccessLayer.DTOs;
using Bookify.Service;
using Bookify.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrdersService _orderService;

        public OrderController(OrdersService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("make-order")]
        public async Task<IActionResult> MakeOrder([FromBody] OrderRequestDTO dto)
        {
            // Get logged-in user ID if available (for authenticated users)
            int? loggedInUserId = null;
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                loggedInUserId = SessionHelper.GetUserId(HttpContext.Session);
            }

            var orderId = await _orderService.MakeOrderAsync(dto, loggedInUserId);
            return Ok(new { OrderId = orderId });
        }


    }
}
