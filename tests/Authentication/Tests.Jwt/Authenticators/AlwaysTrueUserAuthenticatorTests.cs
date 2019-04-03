using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.Authenticators;
using NUnit.Framework;

namespace Tests.Authenticators
{
    [TestFixture]
    public class AlwaysTrueUserAuthenticatorTests
    {
        [Test, CustomAutoData]
        public async Task TryAuthenticateUserAsync_returns_true(AlwaysTrueUserAuthenticator sut, User user)
        {
            var result = await sut.TryAuthenticateUserAsync(user, out _);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public async Task TryAuthenticateUserAsync_outputs_identity_with_username(AlwaysTrueUserAuthenticator sut, User user)
        {
            await sut.TryAuthenticateUserAsync(user, out var identity);

            Assert.That(identity, Is.Not.Null);
            Assert.That(identity.Name, Is.EqualTo(user.UserName));
        }
    }
}
