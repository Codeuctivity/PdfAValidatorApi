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
        public void ShouldDetectNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.False(result);
            }
        }
    }
}