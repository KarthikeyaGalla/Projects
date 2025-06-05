using ECommerceApi.Models;
using ECommerceApi.Helpers;

namespace ECommerceApi.Services
{
    public class UserService : IUserService
    {
        public void Register(RegisterRequest model) 
        {
            // implement registration, hashing, 2FA secret generation
        }
        public AuthenticateResponse Authenticate(LoginRequest model)
        {
            // validate credentials, generate JWT
            return new AuthenticateResponse();
        }
    }
}