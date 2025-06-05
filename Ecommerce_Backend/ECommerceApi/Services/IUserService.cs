using ECommerceApi.Models;

namespace ECommerceApi.Services
{
    public interface IUserService
    {
        void Register(RegisterRequest model);
        AuthenticateResponse Authenticate(LoginRequest model);
    }
}