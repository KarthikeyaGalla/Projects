using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;
using ECommerceApi.Models.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/shophours")]
    public class ShopHoursController : ControllerBase
    {
        private readonly IShopHourService _service;
        public ShopHoursController(IShopHourService service) => _service = service;

        [HttpGet("{shopId}")]
        public IActionResult GetByShop(int shopId) => Ok(_service.GetByShop(shopId));

        [HttpPost]
        public IActionResult Create([FromBody] ShopHourDto dto) => Ok(_service.Create(dto));
    }
}