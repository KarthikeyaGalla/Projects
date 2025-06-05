using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/shops")]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;
        public ShopsController(IShopService shopService) => _shopService = shopService;

        [HttpGet]
        public IActionResult GetAll() => Ok(_shopService.GetAll());
    }
}