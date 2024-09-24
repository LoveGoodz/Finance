using Finance.Models;

namespace Finance.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        string GenerateToken(User user);
    }
}
