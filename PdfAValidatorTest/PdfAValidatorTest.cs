using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace PDfAValidatorTest
{
    public static class PdfAValidatorTest
    {
        [Fact]
        public static void ShouldUnpackNewDirectoryInTempdirectory()
        {
            string veraPdfStartScript;
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                AssertVeraPdfBinCreation(pdfAValidator.VeraPdfStartScript);
            }
            Assert.False(File.Exists(veraPdfStartScript));
        }

        [Fact]
        public static void ShouldDetectCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.True(result);
            }
        }

        [Fact]
        public static void ShouldGetDetailedReportFromPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
                Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
            }
        }

        [Fact]
        public static void ShouldGetDetaileDetaileddReportFromPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
                Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
            }
        }

        [Fact]
        public static void ShouldDetectNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result);
            }
        }

        [Fact]
        public static void ShouldGetDetailedReportFromNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
                Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1B validation profile");
                Assert.True(result.Jobs.Job.ValidationReport.Details.FailedRules == 4);
                Assert.True(result.Jobs.Job.ValidationReport.Details.Rule[0].Clause == "6.7.3");
            }
        }

        [Fact]
        public static void ShouldGetDetailedReportFromNonCompliantPdfAMissingFont()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FontsNotEmbedded.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FontsNotEmbedded.pdf");
                Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
                Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1B validation profile");
                Assert.True(result.Jobs.Job.ValidationReport.Details.Rule.Any(_ => _.Clause == "6.3.5"));
            }
        }

        [Fact]
        public static void ShouldWorkWithCustomJavaAndVeraPdfLocation()
        {
            string veraPdfStartScript;
            // Using default ctor to get verapdf and java bins for the test
            using (var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator())
            {
                {
                    pdfAValidatorPrepareBins.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    using (var pdfAValidator = new PdfAValidator.PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript, pdfAValidatorPrepareBins.PathJava))
                    {
                        veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                        AssertVeraPdfBinCreation(veraPdfStartScript);
                        Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                        var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                        Assert.False(result);
                    }
                    Assert.True(File.Exists(veraPdfStartScript));
                }
            }
            Assert.False(File.Exists(veraPdfStartScript));
        }

        private static void AssertVeraPdfBinCreation(string scriptPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Equal(".bat", scriptPath.Substring(scriptPath.Length - 4));
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Equal("verapdf", scriptPath.Substring(scriptPath.Length - 7));
            }
            Assert.True(File.Exists(scriptPath), scriptPath + " does not exist.");
        }

        [Fact]
        public static void ShouldNotFailOnMultipleDisposeCalls()
        {
            var postscriptValidator = new PdfAValidator.PdfAValidator();
            postscriptValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
            postscriptValidator.Dispose();
            postscriptValidator.Dispose();
        }

        [Fact]
        public static void ShouldNotFailOnMultipleDisposeCallseWithoutInitialization()
        {
            var postscriptValidator = new PdfAValidator.PdfAValidator();
            postscriptValidator.Dispose();
            postscriptValidator.Dispose();
        }
    }
}