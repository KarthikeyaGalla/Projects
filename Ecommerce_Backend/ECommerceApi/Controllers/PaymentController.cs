using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

        [HttpPost("callback")]
        public IActionResult Callback(PaymentCallbackRequest model)
        {
            _paymentService.HandleCallback(model);
            return Ok();
        }
    }
}