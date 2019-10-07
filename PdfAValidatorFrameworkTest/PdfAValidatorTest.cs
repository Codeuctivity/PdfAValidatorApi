using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PdfAValidatorTest
{
    [TestClass]
    public class PdfAValidatorTest
    {
        private string veraPdfStartScript;

        [TestMethod]
        public void ShouldUnpackNewDirectoryInTempdirectory()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                AssertVeraPdfBinCreation(veraPdfStartScript);
            }
            AssertVeraPdfGotDisposed();
        }

        [TestMethod]
        public void ShouldDetectCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void ShouldGetDetailedReportFromPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOffice.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOffice.pdf");
                Assert.IsTrue(result.jobs.job.validationReport.isCompliant);
                Assert.IsTrue(result.jobs.job.validationReport.profileName == "PDF/A-1A validation profile");
            }
        }

        [TestMethod]
        public void ShouldDetectNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void ShouldGetDetailedReportFromNonCompliantPdfA()
        {
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                Assert.IsFalse(result.jobs.job.validationReport.isCompliant);
                Assert.IsTrue(result.jobs.job.validationReport.profileName == "PDF/A-1B validation profile");
            }
        }

        [TestMethod]
        public void ShouldWorkWithCustomJavaAndVeraPdfLocation()
        {
            // Using default ctor to get verapdf and java bins for the test
            using (var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator())
            {
                pdfAValidatorPrepareBins.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                using (var pdfAValidator = new PdfAValidator.PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript, pdfAValidatorPrepareBins.PathJava))
                {
                    AssertVeraPdfBinCreation(pdfAValidator.VeraPdfStartScript);
                    Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                    var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    Assert.IsFalse(result);
                }
                AssertVeraPdfGotDisposed();
            }
        }

        [TestMethod]
        public void ShouldNotFailOnMultipleDisposeCalls()
        {
            var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator();
            pdfAValidatorPrepareBins.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
            veraPdfStartScript = pdfAValidatorPrepareBins.VeraPdfStartScript;
            pdfAValidatorPrepareBins.Dispose();
            AssertVeraPdfGotDisposed();
            pdfAValidatorPrepareBins.Dispose();
        }

        private void AssertVeraPdfGotDisposed()
        {
            Assert.IsFalse(Directory.Exists(veraPdfStartScript));
        }

        [TestMethod]
        public void ShouldNotFailOnMultipleDisposeCallseWithoutInitialization()
        {
            var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator();
            pdfAValidatorPrepareBins.Dispose();
            pdfAValidatorPrepareBins.Dispose();
        }

        private static void AssertVeraPdfBinCreation(string scriptPath)
        {
            Assert.AreEqual(".bat", scriptPath.Substring(scriptPath.Length - 4));
            Assert.IsTrue(File.Exists(scriptPath), scriptPath + " does not exist.");
        }
    }
}