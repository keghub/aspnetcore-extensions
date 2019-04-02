using System.Security.Claims;
using System.Threading.Tasks;

namespace EMG.Extensions.AspNetCore.Authenticators
{
    public class AlwaysTrueUserAuthenticator : IUserAuthenticator
    {
        public Task<bool> TryAuthenticateUserAsync(User user, out ClaimsIdentity identity)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) };

            identity = new ClaimsIdentity(claims);

            return Task.FromResult(true);
        }
    }
}