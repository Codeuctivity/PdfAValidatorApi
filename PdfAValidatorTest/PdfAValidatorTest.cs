using Codeuctivity;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace CodeuctivityTest
{
    public static class PdfAValidatorTest
    {
        [Fact]
        public static async Task ShouldUnpackNewDirectoryInTempdirectory()
        {
            string? veraPdfStartScript;
            using (var pdfAValidator = new PdfAValidator())
            {
                await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
                veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                AssertVeraPdfBinCreation(pdfAValidator.VeraPdfStartScript);
            }
            Assert.False(File.Exists(veraPdfStartScript));
        }

        [Fact]
        public static async Task ShouldDetectCompliantPdfA()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
            Assert.True(result);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromPdfA()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf");
            Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
        }

        [Fact]
        public static async Task ShouldDetectNonCompliantPdfA()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
            var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
            Assert.False(result);
        }

        [Fact]
        public static async Task ShouldDetectNonCompliantPdfAWhenCheckingWrongFlavour()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf", "-f 2b");
            Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-2B validation profile");
        }

        [Fact]
        public static async Task ShouldParseButNotValidateRegularPdfWithValidateOffArgument()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf", "-o");
            Assert.Equal("1", result.BatchSummary.TotalJobs);
            Assert.Equal("0", result.BatchSummary.FailedToParse);
            Assert.Equal("0", result.BatchSummary.Encrypted);
        }

        [Fact]
        public static async Task ShouldThrowOnValidatingBrokenPdf()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/NoPdf.pdf"));
            var actualException = await Assert.ThrowsAsync<VeraPdfException>(() => pdfAValidator.ValidateAsync("./TestPdfFiles/NoPdf.pdf"));
            Assert.Contains("Calling VeraPdf exited with 7 caused an error:", actualException.Message);
            Assert.Contains("NoPdf.pdf doesn't appear to be a valid PDF.", actualException.Message);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromNonCompliantPdfA()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
            Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1B validation profile");
            Assert.InRange(result.Jobs.Job.ValidationReport.Details.FailedRules, 1, 20);
            Assert.Contains(result.Jobs.Job.ValidationReport.Details.Rule, _ => _.Clause == "6.7.3");
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromNonCompliantPdfAMissingFont()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FontsNotEmbedded.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FontsNotEmbedded.pdf");
            Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1B validation profile");
            Assert.Contains(result.Jobs.Job.ValidationReport.Details.Rule, _ => _.Clause == "6.3.5");
        }

        [Fact]
        public static async Task ShouldThrowOnGetDetailedReportFromBrokenPdf()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/NoPdf.pdf"));
            var actualException = await Assert.ThrowsAsync<VeraPdfException>(() => pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/NoPdf.pdf"));
            Assert.Contains("Calling VeraPdf exited with 7 caused an error:", actualException.Message);
            Assert.Contains("NoPdf.pdf doesn't appear to be a valid PDF.", actualException.Message);
        }

        [Fact]
        public static async Task ShouldWorkWithCustomJavaAndVeraPdfLocation()
        {
            string? veraPdfStartScript;
            // Using default ctor to get verapdf and Java bins for the test
            using (var pdfAValidatorPrepareBins = new PdfAValidator())
            {
                await pdfAValidatorPrepareBins.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                using (var pdfAValidator = new PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript!, pdfAValidatorPrepareBins.PathJava!))
                {
                    veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                    AssertVeraPdfBinCreation(veraPdfStartScript);
                    Assert.True(File.Exists("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                    var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    Assert.False(result);
                }
                Assert.True(File.Exists(veraPdfStartScript));
            }
            Assert.False(File.Exists(veraPdfStartScript));
        }

        [Fact]
        public static async Task ShouldFailGracefullWithUnrecognicedVeraPdfOutput()
        {
            var somethingThatReturnsExitcode0 = "./TestExecuteables/exitcode0.bat";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                somethingThatReturnsExitcode0 = "./TestExecuteables/exitcode0.sh";
            }

            var veraPdfException = await Assert.ThrowsAsync<VeraPdfException>(async () =>
              {
                  using var pdfAValidator = new PdfAValidator(somethingThatReturnsExitcode0, "SomeValue");
                  await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
              });

            Assert.Equal($"Failed to parse VeraPdf Output: \nCustom JAVACMD: SomeValue\nveraPdfStartScriptPath: {somethingThatReturnsExitcode0}", veraPdfException.Message);
        }

        [Fact]
        public static async Task ShouldFailGracefullWithExitcode2()
        {
            var somethingThatReturnsExitcode2 = "./TestExecuteables/exitcode2.bat";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                somethingThatReturnsExitcode2 = "./TestExecuteables/exitcode2.sh";
            }

            var veraPdfException = await Assert.ThrowsAsync<VeraPdfException>(async () =>
            {
                // Using default ctor to get Java bins for the test
                using var pdfAValidator = new PdfAValidator(somethingThatReturnsExitcode2, "SomeValue");
                await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
            });

            Assert.Equal($"Calling VeraPdf exited with 2 caused an error: \nCustom JAVACMD: SomeValue\nVeraPdfStartScript: {somethingThatReturnsExitcode2}", veraPdfException.Message);
        }

        private static void AssertVeraPdfBinCreation(string? scriptPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Equal(".bat", scriptPath?.Substring(scriptPath.Length - 4));
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Equal("verapdf", scriptPath?.Substring(scriptPath.Length - 7));
            }
            Assert.True(File.Exists(scriptPath), scriptPath + " does not exist.");
        }

        [Fact]
        public static async Task ShouldNotFailOnMultipleDisposeCalls()
        {
            var postscriptValidator = new PdfAValidator();
            _ = await postscriptValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
            postscriptValidator.Dispose();
#pragma warning disable S3966 // Expecting multiple dispose calls to pass
            postscriptValidator.Dispose();
#pragma warning restore S3966 // Expecting multiple dispose calls to pass
        }

        [Fact]
        public static void ShouldNotFailOnMultipleDisposeCallseWithoutInitialization()
        {
            var postscriptValidator = new PdfAValidator();

            postscriptValidator.Dispose();
#pragma warning disable S3966 // Expecting multiple dispose calls to pass
            postscriptValidator.Dispose();
#pragma warning restore S3966 // Expecting multiple dispose calls to pass

            Assert.True(true);
        }
    }
}