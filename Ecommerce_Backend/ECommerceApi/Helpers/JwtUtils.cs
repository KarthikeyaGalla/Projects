namespace ECommerceApi.Helpers
{
    public interface IJwtUtils { string GenerateToken(int userId); }
    public class JwtUtils : IJwtUtils
    {
        public string GenerateToken(int userId) => "...";
    }
}