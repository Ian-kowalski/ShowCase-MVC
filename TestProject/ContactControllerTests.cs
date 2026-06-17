using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ShowCase_MVC.Controllers;
using System.Net;
using System.Text.Json;

namespace TestProject
{
    [TestFixture]
    public class ContactControllerTests
    {
        private IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Resend:ApiKey"] = "test-key",
                    ["Resend:From"] = "from@example.com",
                    ["Resend:To"] = "to@example.com"
                })
                .Build();

        private ContactController BuildController(HttpStatusCode statusCode, string responseBody)
        {
            var handler = new MockHttpMessageHandler(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody)
            });
            var httpClient = new HttpClient(handler);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return new ContactController(BuildConfig(), factory.Object);
        }

        private static ContactRequest SampleRequest() =>
            new("visitor@example.com", "Hallo", "Jan", "Janssen", "0612345678", "Test bericht");

        [Test]
        public async Task Post_WhenResendSucceeds_ReturnsSuccessTrue()
        {
            var controller = BuildController(HttpStatusCode.OK, "{\"id\":\"abc123\"}");

            var result = await controller.Post(SampleRequest()) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var json = JsonSerializer.Serialize(result!.Value);
            Assert.That(json, Does.Contain("\"success\":true"));
        }

        [Test]
        public async Task Post_WhenResendFails_ReturnsSuccessFalse()
        {
            var controller = BuildController(HttpStatusCode.Unauthorized, "{\"message\":\"Invalid API key\"}");

            var result = await controller.Post(SampleRequest()) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var json = JsonSerializer.Serialize(result!.Value);
            Assert.That(json, Does.Contain("\"success\":false"));
        }

        [Test]
        public async Task Post_WhenResendFails_IncludesErrorInMessage()
        {
            var controller = BuildController(HttpStatusCode.Unauthorized, "Invalid API key");

            var result = await controller.Post(SampleRequest()) as OkObjectResult;

            var json = JsonSerializer.Serialize(result!.Value);
            Assert.That(json, Does.Contain("Resend fout"));
        }
    }

    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public MockHttpMessageHandler(HttpResponseMessage response) => _response = response;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }
}
