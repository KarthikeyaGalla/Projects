using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public IActionResult Place(OrderRequest model) => Ok(_orderService.Place(model));

        [HttpGet("user/{userId}")]
        public IActionResult GetForUser(int userId) => Ok(_orderService.GetByUser(userId));
    }
}