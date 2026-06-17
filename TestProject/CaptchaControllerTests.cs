using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ShowCase_MVC.Controllers;
using System.Net;
using System.Text.Json;

namespace TestProject
{
    [TestFixture]
    public class CaptchaControllerTests
    {
        private IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Recaptcha:SecretKey"] = "test-secret"
                })
                .Build();

        private CaptchaController BuildController(string googleResponse)
        {
            var handler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(googleResponse)
            });
            var httpClient = new HttpClient(handler);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return new CaptchaController(BuildConfig(), factory.Object);
        }

        [Test]
        public async Task Post_ReturnsScoreFromGoogle()
        {
            var controller = BuildController("{\"success\":true,\"score\":0.9}");

            var result = await controller.Post(new CaptchaRequest("token")) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var json = JsonSerializer.Serialize(result!.Value);
            Assert.That(json, Does.Contain("0.9"));
        }

        [Test]
        public async Task Post_ReturnsZeroScore_WhenGoogleResponseHasNoScore()
        {
            var controller = BuildController("{\"success\":false}");

            var result = await controller.Post(new CaptchaRequest("bad-token")) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var json = JsonSerializer.Serialize(result!.Value);
            Assert.That(json, Does.Contain("\"score\":0"));
        }

        [Test]
        public async Task Post_ReturnsOkResult_Always()
        {
            var controller = BuildController("{\"success\":true,\"score\":0.5}");

            var result = await controller.Post(new CaptchaRequest("token"));

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
