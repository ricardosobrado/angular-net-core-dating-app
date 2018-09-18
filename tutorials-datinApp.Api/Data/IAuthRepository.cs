using System.Threading.Tasks;
using tutorials_datinApp.Api.Models;

namespace tutorials_datinApp.Api.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);

        Task<User> Login(string username, string password);

        Task<bool> UserExists(string username);
    }
}