using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace PdfAValidatorTest
{
    [TestClass]
    public class PdfAValidatorTest
    {
        [TestMethod]
        public void ShouldUnpackNewDirectoryInTempdirectoryFramework()
        {
            var listOfDirectoriesInTempWithoutVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                AssertVeraPdfBinCreation(listOfDirectoriesInTempWithoutVeraPdf, pdfAValidator);
            }
            var listOfDirectoriesInTempAfterVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            Assert.AreEqual(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);
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
            var listOfDirectoriesInTempWithoutVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            using (var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator())
            {
                {
                    pdfAValidatorPrepareBins.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    using (var pdfAValidator = new PdfAValidator.PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript, pdfAValidatorPrepareBins.PathJava))
                    {
                        AssertVeraPdfBinCreation(listOfDirectoriesInTempWithoutVeraPdf, pdfAValidator);
                        Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                        var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                        Assert.IsFalse(result);
                    }
                }
            }
            var listOfDirectoriesInTempAfterVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            Assert.AreEqual(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);
        }

        private static void AssertVeraPdfBinCreation(string[] listOfDirectoriesInTempWithoutVeraPdf, PdfAValidator.PdfAValidator pdfAValidator)
        {
            var listOfDirectoriesInTempWithVeraPdf = Directory.GetDirectories(Path.GetTempPath());
            var newDirectories = listOfDirectoriesInTempWithVeraPdf.Except(listOfDirectoriesInTempWithoutVeraPdf);

            Assert.AreEqual(newDirectories.Count(), 1);
            var scriptPath = pdfAValidator.VeraPdfStartScript;
            Assert.AreEqual(".bat", scriptPath.Substring(scriptPath.Length - 4));
            Assert.IsTrue(File.Exists(scriptPath), scriptPath + " does not exist.");
        }
    }
}