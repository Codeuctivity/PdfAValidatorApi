using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace PDfAValidatorTest
{
    public class PdfAValidatorTest
    {
        [Fact]
        public void ShouldUnpackNewDirectoryInTempdirectory()
        {
            var listOfDirectoriesInTempWithoutVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                var listOfDirectoriesInTempWithVeraPdf = Directory.GetDirectories(Path.GetTempPath());
                var newDirectories = listOfDirectoriesInTempWithVeraPdf.Except(listOfDirectoriesInTempWithoutVeraPdf);

                Assert.Equal(1, newDirectories.Count());
                var scriptPath = pdfAValidator.VeraPdfStarterScript;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(".bat", scriptPath.Substring(scriptPath.Length - 4));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Assert.Equal("verapdf", scriptPath.Substring(scriptPath.Length - 7));

                Assert.True(File.Exists(scriptPath), scriptPath + " does not exist.");
            }
            var listOfDirectoriesInTempAfterVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            Assert.Equal(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);

        }

        [Fact]
        public void ShouldDetectCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.True(result);
            }
        }

        [Fact]
        public void ShouldGetDetailedReportFromPdfA()
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
        public void ShouldDetectNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result);
            }
        }

        [Fact]
        public void ShouldGetDetailedReportFromNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.True(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result.jobs.job.validationReport.isCompliant);
                Assert.True(result.jobs.job.validationReport.profileName == "PDF/A-1B validation profile");
            }
        }
    }
}