using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace PdfAValidatorTest
{
    [TestClass]
    public class PdfAValidatorTest
    {
        private string tempPath;

        [TestInitialize]
        public void InitialThings()
        { tempPath = Path.GetTempPath(); }

        [TestMethod]
        public void ShouldUnpackNewDirectoryInTempdirectory()
        {
            var listOfDirectoriesInTempWithoutVeraPdf = GetListOfDirectoriesInTempExceptDirectoriesCreatedByAppVeyor();
            using (var pdfAValidator = new PdfAValidator.PdfAValidator())
            {
                pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
                AssertVeraPdfBinCreation(pdfAValidator.VeraPdfStartScript);
            }
            var listOfDirectoriesInTempAfterVeraPdf = GetListOfDirectoriesInTempExceptDirectoriesCreatedByAppVeyor();
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
            var listOfDirectoriesInTempWithoutVeraPdf = GetListOfDirectoriesInTempExceptDirectoriesCreatedByAppVeyor();
            using (var pdfAValidatorPrepareBins = new PdfAValidator.PdfAValidator())
            {
                {
                    pdfAValidatorPrepareBins.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    using (var pdfAValidator = new PdfAValidator.PdfAValidator(pdfAValidatorPrepareBins.VeraPdfStartScript, pdfAValidatorPrepareBins.PathJava))
                    {
                        AssertVeraPdfBinCreation(pdfAValidator.VeraPdfStartScript);
                        Assert.IsTrue(File.Exists(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"));
                        var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                        Assert.IsFalse(result);
                    }
                }
            }
            var listOfDirectoriesInTempAfterVeraPdf = GetListOfDirectoriesInTempExceptDirectoriesCreatedByAppVeyor();
            Assert.AreEqual(listOfDirectoriesInTempAfterVeraPdf.Length, listOfDirectoriesInTempWithoutVeraPdf.Length);
        }

        private static void AssertVeraPdfBinCreation(string scriptPath)
        {
            Assert.AreEqual(".bat", scriptPath.Substring(scriptPath.Length - 4));
            Assert.IsTrue(File.Exists(scriptPath), scriptPath + " does not exist.");
        }

        private string[] GetListOfDirectoriesInTempExceptDirectoriesCreatedByAppVeyor()
        {
            return Directory.GetDirectories(tempPath).Where(_ => !_.Contains("appveyor"))?.ToArray();
        }
    }
}