using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;
using ECommerceApi.Models.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/shopreviews")]
    public class ShopReviewsController : ControllerBase
    {
        private readonly IShopReviewService _service;
        public ShopReviewsController(IShopReviewService service) => _service = service;

        [HttpGet("{shopId}")]
        public IActionResult GetByShop(int shopId) => Ok(_service.GetByShop(shopId));

        [HttpPost]
        public IActionResult Create([FromBody] ShopReviewDto dto) => Ok(_service.Create(dto));
    }
}