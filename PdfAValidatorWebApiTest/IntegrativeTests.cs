using Codeuctivity.PdfAValidatorWebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using PdfAValidatorWebApi;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CodeuctivityWebApiTest
{
    // Note: not passing using wsl
    public class IntegrativeTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string baseUrl = "http://localhost";
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrativeTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/", "text/html; charset=utf-8")]
        [InlineData("/swagger/v1/swagger.json", "application/json; charset=utf-8")]
        public async Task ShouldAccessEndpointSuccessful(string route, string contentType)
        {
            // Arrange
            var client = _factory.CreateClient();
            var expectedUrl = new Uri($"https://localhost{route}");

            // Act
            var response = await client.GetAsync(expectedUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(contentType, response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task ShouldDownloadOpenApiDescription()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("http://localhost/swagger/v1/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode();
            var actual = await response.Content.ReadAsStringAsync();

            // use to update reference
            // await File.WriteAllTextAsync("../../../OpenApi.json", actual);

            AssertOpenApiJsonEqualsExpected(actual);
        }

        private static void AssertOpenApiJsonEqualsExpected(string actual)
        {
            var assemblyVersion = typeof(Program).Assembly.GetName().Version;
            var expected = File.ReadAllText("../../../OpenApi.json").Replace("PdfAValidator 0.0.1.0", $"PdfAValidator {assemblyVersion}");
            expected = expected.ReplaceLineEndings();
            actual = actual.ReplaceLineEndings();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ShouldValidatePdf()
        {
            // Arrange
            var httpClient = _factory.CreateClient();

            using var request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost/api/PdfAValidator");
            using var file = new ByteArrayContent(File.ReadAllBytes("../../../FromLibreOffice.pdf"));
            file.Headers.Add("Content-Type", "application/pdf");
            var multiPartContent = new MultipartFormDataContent
            {
                { file, "pdfFile", Path.GetFileName("FromLibreOffice.pdf") }
            };
            request.Content = multiPartContent;

            // Act
            var response = await httpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var actual = await response.Content.ReadAsStringAsync();
            Assert.Equal("true", actual);
        }

        [Fact]
        public async Task ShouldValidatePdfOpenApiClient()
        {
            // Arrange
            var httpClient = _factory.CreateClient();

            var openApiClient = new Client(baseUrl, httpClient);

            using var fileStream = new FileStream("../../../FromLibreOffice.pdf", FileMode.Open, FileAccess.Read);
            var pdfFile = new FileParameter(fileStream);

            // Act
            var response = await openApiClient.PdfAValidatorAsync(pdfFile);

            // Assert
            Assert.True(response);
        }

        [Fact]
        public async Task ShouldValidatePdfDetailedReport()
        {
            // Arrange
            var client = _factory.CreateClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost/api/PdfAValidator/DetailedReport");
            using var file = new ByteArrayContent(File.ReadAllBytes("../../../FromLibreOffice.pdf"));
            file.Headers.Add("Content-Type", "application/pdf");
            var multiPartContent = new MultipartFormDataContent
            {
                { file, "pdfFile", Path.GetFileName("FromLibreOffice.pdf") }
            };
            request.Content = multiPartContent;

            // Act
            var response = await client.SendAsync(request);
            // Assert
            response.EnsureSuccessStatusCode();

            var actual = await response.Content.ReadAsStringAsync();
            Assert.Contains("isCompliant\":true", actual);
        }

        [Fact]
        public async Task ShouldValidatePdfDetailedReportOpenApiClient()
        {
            // Arrange
            var httpClient = _factory.CreateClient();

            var openApiClient = new Client(baseUrl, httpClient);

            using var fileStream = new FileStream("../../../FromLibreOffice.pdf", FileMode.Open, FileAccess.Read);
            var pdfFile = new FileParameter(fileStream);

            // Act
            var actual = await openApiClient.DetailedReportAsync(pdfFile);

            // Assert
            Assert.True(actual.Jobs.AllJobs.Single().ValidationReport.IsCompliant);
        }
    }
}