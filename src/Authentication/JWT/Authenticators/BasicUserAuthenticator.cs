using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EMG.Extensions.AspNetCore.Authenticators
{
    public class BasicUserAuthenticator : IUserAuthenticator
    {
        private readonly BasicCredentials _pair;

        public BasicUserAuthenticator(BasicCredentials pair)
        {
            _pair = pair ?? throw new ArgumentNullException(nameof(pair));
        }

        public Task<bool> TryAuthenticateUserAsync(User user, out ClaimsIdentity identity)
        {
            identity = null;

            if (user.UserName == _pair.Username && user.Password == _pair.Password)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) };

                identity = new ClaimsIdentity(claims);

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }

    public class BasicCredentials
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}