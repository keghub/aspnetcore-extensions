using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using EMG.Extensions.AspNetCore.Authenticators;
using EMG.Extensions.AspNetCore.UserExtractors;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EMG.Extensions.AspNetCore
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserAuthenticator _authenticator;
        private readonly IEnumerable<IUserExtractor> _userExtractors;

        public JwtMiddleware(RequestDelegate next, IUserAuthenticator authenticator, IEnumerable<IUserExtractor> userExtractors)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
            _userExtractors = userExtractors ?? throw new ArgumentNullException(nameof(userExtractors));
        }

        public async Task Invoke(HttpContext context, JwtOptions options)
        {
            if (TryGetUserFromRequest(context, out var user) && await _authenticator.TryAuthenticateUserAsync(user, out var identity))
            {
                var claims = new List<Claim>(identity.Claims)
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, await options.IssuerOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, options.IssuerOptions.IssuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };


                var jwt = new JwtSecurityToken(
                    issuer: options.IssuerOptions.Issuer,
                    audience: options.IssuerOptions.Audience,
                    claims: claims,
                    notBefore: options.IssuerOptions.NotBefore.LocalDateTime,
                    expires: options.IssuerOptions.Expiration.LocalDateTime,
                    signingCredentials: options.SigningCredentials
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new TokenModel
                {
                    AccessToken = encodedJwt,
                    ExpiresIn = (int)options.IssuerOptions.ValidFor.TotalSeconds
                };

                await context.WriteModelAsync(response);

                return;
            }

            await context.WriteModelAsync(new { message = "Impossible to authenticate the request" }, HttpStatusCode.Unauthorized);
        }

        private bool TryGetUserFromRequest(HttpContext context, out User user)
        {
            user = null;

            foreach (var extractor in _userExtractors)
            {
                if (extractor.TryExtractUser(context, out user))
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}