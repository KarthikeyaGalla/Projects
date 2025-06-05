using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;
using ECommerceApi.Models.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;
        public CartController(ICartService service) => _service = service;

        [HttpGet("{customerId}")]
        public IActionResult GetByCustomer(int customerId) => Ok(_service.GetByCustomer(customerId));

        [HttpPost]
        public IActionResult Add([FromBody] CartItemDto dto) => Ok(_service.Add(dto));
    }
}