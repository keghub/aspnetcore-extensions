using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        [Test, AutoData]
        public async Task Get_returns_401_if_no_token_is_present(TestWebApplicationFactory factory, string url)
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync($"/{url}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test, AutoData]
        public async Task Get_returns_200_if_authenticated(TestWebApplicationFactory factory, string url)
        {
            var client = factory.CreateClient();

            string token = null;

            using (var authRequest = new HttpRequestMessage(HttpMethod.Post, factory.AuthenticationEndpoint))
            {
                authRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["username"] = factory.Username,
                    ["password"] = factory.Password
                });

                using (var authResponse = await client.SendAsync(authRequest))
                {
                    authResponse.EnsureSuccessStatusCode();

                    var body = await authResponse.Content.ReadAsStringAsync();

                    var obj = JObject.Parse(body);

                    token = obj["access_token"].Value<string>();
                }
            }

            Assume.That(token, Is.Not.Null);

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/{url}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("BEARER", token);

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }
            }
        }
    }
}