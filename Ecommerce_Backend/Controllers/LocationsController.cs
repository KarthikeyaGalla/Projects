using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _service;
        public LocationsController(ILocationService service) => _service = service;

        [HttpGet("{entityType}/{entityId}")]
        public IActionResult Get(string entityType, int entityId) => Ok(_service.Get(entityType, entityId));
    }
}