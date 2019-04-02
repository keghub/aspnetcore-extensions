using System.Security.Claims;
using System.Threading.Tasks;

namespace EMG.Extensions.AspNetCore.Authenticators
{
    public interface IUserAuthenticator
    {
        Task<bool> TryAuthenticateUserAsync(User user, out ClaimsIdentity identity);
    }
}