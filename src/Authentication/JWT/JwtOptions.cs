using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using EMG.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace EMG.Extensions.AspNetCore
{
    public class JwtOptions
    {
        [Required]
        public string AuthenticationEndpoint { get; set; } = "/auth/jwt";

        [Required]
        public string SecretKey { get; set; }

        [Required]
        public JwtIssuerOptions IssuerOptions { get; set; }

        public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public SigningCredentials SigningCredentials => new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        public bool ValidateIssuer { get; set; } = true;

        public bool ValidateAudience { get; set; } = true;

        public bool ValidateIssuerSigningKey { get; set; } = true;

        public bool RequireExpirationTime { get; set; } = true;

        public bool ValidateLifetime { get; set; } = true;

        public TimeSpan AllowedClockSkew { get; set; } = TimeSpan.Zero;
    }

    public class JwtIssuerOptions
    {
        [Required]
        public string Issuer { get; set; }

        public string Subject { get; set; }

        [Required]
        public string Audience { get; set; }

        public DateTimeOffset NotBefore => Clock.Default.UtcNow.AddMinutes(-1);

        public DateTimeOffset IssuedAt => Clock.Default.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(3);

        public DateTimeOffset Expiration => IssuedAt.Add(ValidFor);

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
    }
}