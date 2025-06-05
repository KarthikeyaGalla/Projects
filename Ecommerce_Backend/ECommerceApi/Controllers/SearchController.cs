using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly IShopService _shopService;
        public SearchController(IShopService shopService) => _shopService = shopService;

        [HttpGet("shop/{shopId}")]
        public IActionResult GetShop(int shopId) => Ok(_shopService.GetById(shopId));
    }
}