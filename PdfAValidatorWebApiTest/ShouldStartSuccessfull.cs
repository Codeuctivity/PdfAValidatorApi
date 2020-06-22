using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CodeuctivityWebApiTest
{
    public class ShouldStartSuccessfull : IClassFixture<WebApplicationFactory<CodeuctivityWebApi.Startup>>
    {
        private readonly WebApplicationFactory<CodeuctivityWebApi.Startup> _factory;

        public ShouldStartSuccessfull(WebApplicationFactory<CodeuctivityWebApi.Startup> factory)
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

        [Fact]
        public async Task ShouldValidatePdf()
        {
            // Arrange
            var client = _factory.CreateClient();

            using var request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost/api/PdfAValidator");
            request.Headers.TryAddWithoutValidation("accept", "*/*");

            var multipartContent = new MultipartFormDataContent();
            var file1 = new ByteArrayContent(File.ReadAllBytes("../../../FromLibreOffice.pdf"));
            file1.Headers.Add("Content-Type", "application/pdf");
            multipartContent.Add(file1, "pdfFile", Path.GetFileName("FromLibreOffice.pdf"));
            request.Content = multipartContent;

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}