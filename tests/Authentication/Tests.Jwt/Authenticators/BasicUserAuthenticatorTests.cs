using System.Threading.Tasks;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.Authenticators;
using NUnit.Framework;

namespace Tests.Authenticators
{
    [TestFixture]
    public class BasicUserAuthenticatorTests
    {
        [Test, CustomAutoData]
        public async Task TryAuthenticateUserAsync_returns_true_on_match([Frozen] BasicCredentials credentials, BasicUserAuthenticator sut)
        {
            var user = new User
            {
                UserName = credentials.Username,
                Password = credentials.Password
            };

            var result = await sut.TryAuthenticateUserAsync(user, out _);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public async Task TryAuthenticateUserAsync_outputs_identity_with_username([Frozen] BasicCredentials credentials, BasicUserAuthenticator sut)
        {
            var user = new User
            {
                UserName = credentials.Username,
                Password = credentials.Password
            };

            await sut.TryAuthenticateUserAsync(user, out var identity);

            Assert.That(identity, Is.Not.Null);
            Assert.That(identity.Name, Is.EqualTo(user.UserName));
        }

        [Test, CustomAutoData]
        public async Task TryAuthenticateUserAsync_returns_false_on_no_match([Frozen] BasicCredentials credentials, BasicUserAuthenticator sut, User user)
        {
            var result = await sut.TryAuthenticateUserAsync(user, out _);

            Assert.That(result, Is.False);
        }
    }
}