using Learntendo_backend.Models;

namespace Learntendo_backend.Data
{
    public interface IAuthRepository
    {
        
        Task<User> Register(User user, string password);
        Task<User> Login(string email, string password);
        Task<Admin> LoginAdmin(string email, string password);
        Task<bool> UserExist(string username);
    }
}
