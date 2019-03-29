using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore.Filters;
using Error;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests
{
    public class ValuesControllerTests
    {
        [Test, AutoData]
        public async Task ExceptionHandlerFilter_sets_statusCode_to_500(WebApplicationFactory<Startup> factory, int id)
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync($"/api/values/{id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test, AutoData]
        public async Task ExceptionHandlerFilter_sets_statusCode_to_500(WebApplicationFactory<Startup> factory)
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/values");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test, AutoData]
        public async Task ExceptionHandlerFilter_returns_error_model(WebApplicationFactory<Startup> factory, int id)
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync($"/api/values/{id}");

            var responseBody = await response.Content.ReadAsStringAsync();

            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseBody);

            Assert.That(errorModel, Is.Not.Null);

            var jo = errorModel.Data as JObject;

            Assert.That(jo, Is.Not.Null);
            Assert.That(jo["id"].Value<int>(), Is.EqualTo(id));
        }

        [Test, AutoData]
        public async Task ExceptionHandlerFilter_returns_error_model(WebApplicationFactory<Startup> factory)
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/values");

            var responseBody = await response.Content.ReadAsStringAsync();

            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseBody);

            Assert.That(errorModel, Is.Not.Null);
        }
    }
}