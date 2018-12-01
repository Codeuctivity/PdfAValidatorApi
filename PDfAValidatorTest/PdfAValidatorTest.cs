using Xunit;

namespace PDfAValidatorTest
{
    public class PdfAValidatorTest
    {
        [Fact]
        public void ShouldDetectCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.True(result);
            }
        }

        [Fact]
        public void ShouldGetDetailedReportFromPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
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
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result);
            }
        }

        [Fact]
        public void ShouldGetDetailedReportFromNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result.jobs.job.validationReport.isCompliant);
                Assert.True(result.jobs.job.validationReport.profileName == "PDF/A-1B validation profile");
            }
        }
    }
}