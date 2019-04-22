using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace PDfAValidatorTest
{
    public class PdfAValidatorTest
    {
        [Fact]
        public static void ShouldUnpackNewDirectoryInTempdirectory()
        {
            var listOfDirectoriesInTempWithoutVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                AssertVeraPdfBinCreation(listOfDirectoriesInTempWithoutVeraPdf, pdfAValidator);
            }
            var listOfDirectoriesInTempAfterVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            Assert.Equal(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);
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
                Assert.True(result.jobs.job.validationReport.isCompliant);
                Assert.True(result.jobs.job.validationReport.profileName == "PDF/A-1A validation profile");
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
                Assert.False(result.jobs.job.validationReport.isCompliant);
                Assert.True(result.jobs.job.validationReport.profileName == "PDF/A-1B validation profile");
            }
        }

        [Fact]
        public static void ShouldWorkWithCustomJavaAndVeraPdfLocation()
        {
            // Using default ctor to get verapdf and java bins for the test
            var listOfDirectoriesInTempWithoutVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            using (var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator())
            {
                using (var pdfAValidator = new PdfAValidator.PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript, pdfAValidatorPrepareBins.PathJava))
                {
                    AssertVeraPdfBinCreation(listOfDirectoriesInTempWithoutVeraPdf, pdfAValidator);
                    Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                    var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    Assert.False(result);
                }
            }
            var listOfDirectoriesInTempAfterVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            Assert.Equal(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);
        }

        private static void AssertVeraPdfBinCreation(string[] listOfDirectoriesInTempWithoutVeraPdf, PdfAValidator.PdfAValidator pdfAValidator)
        {
            var listOfDirectoriesInTempWithVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            var newDirectories = listOfDirectoriesInTempWithVeraPdf.Except(listOfDirectoriesInTempWithoutVeraPdf);

            Assert.Single(newDirectories);
            var scriptPath = pdfAValidator.VeraPdfStartScript;
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
    }
}