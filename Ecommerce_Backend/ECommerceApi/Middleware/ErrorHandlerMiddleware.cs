using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace ECommerceApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;
        public async Task Invoke(HttpContext context) { await _next(context); }
    }
}