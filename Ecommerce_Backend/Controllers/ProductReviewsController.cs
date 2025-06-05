using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;
using ECommerceApi.Models.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/productreviews")]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _service;
        public ProductReviewsController(IProductReviewService service) => _service = service;

        [HttpGet("{productId}")]
        public IActionResult GetByProduct(int productId) => Ok(_service.GetByProduct(productId));

        [HttpPost]
        public IActionResult Create([FromBody] ProductReviewDto dto) => Ok(_service.Create(dto));
    }
}