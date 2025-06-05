using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;
using ECommerceApi.Models.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) => _service = service;

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpPost]
        public IActionResult Create([FromBody] CategoryDto dto) => Ok(_service.Create(dto));
    }
}