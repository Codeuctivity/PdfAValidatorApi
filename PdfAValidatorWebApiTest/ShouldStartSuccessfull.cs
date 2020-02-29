using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PdfAValidatorWebApiTest
{
    public class ShouldStartSuccessfull : IClassFixture<WebApplicationFactory<PdfAValidatorWebApi.Startup>>
    {
        private readonly WebApplicationFactory<PdfAValidatorWebApi.Startup> _factory;

        public ShouldStartSuccessfull(WebApplicationFactory<PdfAValidatorWebApi.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/", "text/html; charset=utf-8")]
        [InlineData("/swagger/v1/swagger.json", "application/json; charset=utf-8")]
        public async Task ShouldAccessEndpointSuccessfull(string route, string contentType)
        {
            // Arrange
            var client = _factory.CreateClient();
            var expectedUrl = new Uri($"https://localhost{route}");

            // Act
            var response = await client.GetAsync(expectedUrl).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(contentType,
                response.Content.Headers.ContentType.ToString());
        }
    }
}