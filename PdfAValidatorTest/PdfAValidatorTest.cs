using Codeuctivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        public static async Task ShouldDetectNonCompliantPdfA2()
        {
            using var pdfAValidator = new PdfAValidator();
            var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/PdfNonCompliant.pdf");
            Assert.False(result);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromPdfA()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf");
            Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
            Assert.StartsWith(@"<?xml version=""1.0"" encoding=""utf-8""?>", result.RawOutput);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromPdfA2()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/PdfNonCompliant.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/PdfNonCompliant.pdf");
            Assert.False(result.Jobs.Job.ValidationReport.IsCompliant);
            Assert.Equal("PDF/A-1B validation profile", result.Jobs.Job.ValidationReport.ProfileName);
            Assert.StartsWith(@"<?xml version=""1.0"" encoding=""utf-8""?>", result.RawOutput);
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
            var taskResult = result.Jobs.Job.TaskResult;
            Assert.Equal(string.Empty, taskResult.Type);
            Assert.Equal(string.Empty, taskResult.ExceptionMessage);
        }

        [Fact]
        public static async Task ShouldDetectBrokenPdfAsNonCompliant()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/NoPdf.pdf"));
            var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/NoPdf.pdf");
            Assert.False(result);
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
        public static async Task ShouldGetDetailedReportFromBrokenPdf()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/NoPdf.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/NoPdf.pdf");
            var taskResult = result.Jobs.Job.TaskResult;
            Assert.True(taskResult.IsExecuted);
            Assert.False(taskResult.IsSuccess);
            Assert.Equal("PARSE", taskResult.Type);
            Assert.Contains("Couldn't parse", taskResult.ExceptionMessage);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromCompliantPdfExceptPdfXConformance()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/PdfxConformancePdfxNone.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/PdfxConformancePdfxNone.pdf");
            var taskResult = result.Jobs.Job.TaskResult;
            Assert.False(taskResult.IsSuccess);
            Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1B validation profile");
            Assert.InRange(result.Jobs.Job.ValidationReport.Details.FailedRules, 1, 1);
            Assert.Contains(result.Jobs.Job.ValidationReport.Details.Rule, _ => _.Clause == "6.7.11");
            Assert.Contains(result.Jobs.Job.ValidationReport.Details.Rule, _ => _.Description == "The PDF/A version and conformance level of a file shall be specified using the PDF/A Identification extension schema.");
        }

        [Fact]
        public static async Task ShouldGetCorrectFileNameWithUnicodeChars()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/NoPdfWithUnicodeChars-üåäö.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/NoPdfWithUnicodeChars-üåäö.pdf");
            Assert.Contains("NoPdfWithUnicodeChars-üåäö.pdf", result.Jobs.Job.Item.Name);
        }

        [Fact]
        public static async Task ShouldGetDetailedReportFromEncryptedPdf()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/Encrypted.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/Encrypted.pdf");
            var taskResult = result.Jobs.Job.TaskResult;
            Assert.True(taskResult.IsExecuted);
            Assert.False(taskResult.IsSuccess);
            Assert.Equal("PARSE", taskResult.Type);
            Assert.Contains("appears to be encrypted", taskResult.ExceptionMessage);
        }

        [Fact]
        public static async Task ShouldValidateFolderWithExpectedResult()
        {
            using var pdfAValidator = new PdfAValidator();
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles", "");

            Assert.Equal("8", result.BatchSummary.TotalJobs);
            Assert.Equal(8, result.Jobs.AllJobs.Count);
            Assert.Equal("1", result.BatchSummary.ValidationReports.Compliant);
            Assert.Equal("4", result.BatchSummary.ValidationReports.NonCompliant);
            Assert.Equal("3", result.BatchSummary.ValidationReports.FailedJobs);
        }

        [Fact]
        public static async Task ShouldGetFeaturesReportWhenAskingForIt()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf", "--extract informationDict");
            var producerEntry = result.Jobs.Job.FeaturesReport.InformationDict.Entries.Single(e => e.Key == "Producer");
            Assert.Equal("LibreOffice 6.1", producerEntry.Value);
        }

        [Fact]
        public static async Task ShouldNotGetFeaturesReportWhenNotAskingForIt()
        {
            using var pdfAValidator = new PdfAValidator();
            Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
            var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf");
            Assert.Empty(result.Jobs.Job.FeaturesReport.InformationDict.Entries);
        }

        [Fact]
        public static async Task ShouldNotDeadlockWhenValidatingFolderThatCausesLargeOutput()
        {
            // Setup folder with pdf files that causes large output streams
            string tmpDirName = "./TestPdfFilesTmp";
            if (Directory.Exists(tmpDirName))
            {
                Directory.Delete(tmpDirName, true);
            }
            Directory.CreateDirectory(tmpDirName);
            var sourceDir = new DirectoryInfo("./TestPdfFiles");
            foreach (var file in sourceDir.GetFiles())
            {
                for (int i = 0; i <= 9; i++)
                {
                    file.CopyTo(Path.Combine(tmpDirName, $"{file.Name}{i}.pdf"));
                }
            }

            var pdfAValidator = new PdfAValidator();
            var task = pdfAValidator.ValidateWithDetailedReportAsync(tmpDirName, "");

            bool completedTask = task.Wait(TimeSpan.FromMinutes(1));
            Assert.True(completedTask);
            if (completedTask)
            {
                var result = await task;
                Assert.Equal("80", result.BatchSummary.TotalJobs);
            }
        }

        [Fact]
        public static async Task ShouldBatchValidatePdfsWithoutThrowingException()
        {
            using var pdfAValidator = new PdfAValidator();
            var files = new[]
            {
                "./TestPdfFiles/FromLibreOffice.pdf",
                "./TestPdfFiles/FromLibreOfficeNonPdfA.pdf"
            };
            Assert.True(files.All(f => File.Exists(f)));
            var result = await pdfAValidator.ValidateBatchWithDetailedReportAsync(files, "");

            Assert.Equal("2", result.BatchSummary.TotalJobs);
            Assert.Equal(2, result.Jobs.AllJobs.Count);
            Assert.Equal("1", result.BatchSummary.ValidationReports.Compliant);
            Assert.Equal("1", result.BatchSummary.ValidationReports.NonCompliant);
            var fromLibreOfficeJob = result.Jobs.AllJobs[0];
            Assert.True(fromLibreOfficeJob.ValidationReport.IsCompliant);
            var nonPdfAJob = result.Jobs.AllJobs[1];
            Assert.False(nonPdfAJob.ValidationReport.IsCompliant);
        }

        [Fact]
        public static async Task ShouldBatchValidatePdfsAndThrowOnMissingPDFs()
        {
            using var pdfAValidator = new PdfAValidator();
            const string invalidPath = "DoesNotExist.pdf";
            var files = new[]
            {
                "./TestPdfFiles/FromLibreOfficeNonPdfA.pdf",
                invalidPath
            };
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
                 { return pdfAValidator.ValidateBatchWithDetailedReportAsync(files, ""); });

            Assert.Contains(invalidPath, exception.Message);
        }

        [Fact]
        public static async Task ShouldThrowPathTooLongException()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var initialEnvornmentTmpValue = Environment.GetEnvironmentVariable("TMP");

            try
            {
                var realyLongPath = Path.Combine(initialEnvornmentTmpValue!, "RealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyLongRealyRealyLongRealy", Guid.NewGuid().ToString());

                Environment.SetEnvironmentVariable("TMP", realyLongPath);

                using var pdfAValidator = new PdfAValidator();
                var files = new[] { "./TestPdfFiles/FromLibreOffice.pdf", "./TestPdfFiles/FromLibreOfficeNonPdfA.pdf" };
                _ = await Assert.ThrowsAsync<PathTooLongException>(() =>
                {
                    return pdfAValidator.ValidateBatchWithDetailedReportAsync(files, "");
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable("TMP", initialEnvornmentTmpValue);
            }
        }

        [Fact]
        public static async Task ShouldThrowExplainableExceptionOnTooLongCommandLineOnWindows()
        {
            var expectedLocalizedMessage = "The command line is too long.";

            if (CultureInfo.InstalledUICulture.Name.StartsWith("de"))
            {
                expectedLocalizedMessage = "Die Befehlszeile ist zu lang.";
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            using var pdfAValidator = new PdfAValidator();
            var tooLongFileList = new List<string>();
            var path = Path.GetFullPath("./TestPdfFiles/FromLibreOffice.pdf");
            var necessaryNumberOfFiles = 10000 / path.Length;

            for (var i = 1; i <= necessaryNumberOfFiles; i++)
            {
                tooLongFileList.Add(path);
            }

            var actualException = await Assert.ThrowsAsync<VeraPdfException>(() => pdfAValidator.ValidateBatchWithDetailedReportAsync(tooLongFileList, string.Empty));
            Assert.Contains("Calling VeraPdf exited with 1 without any output.", actualException.Message);
            Assert.Contains(expectedLocalizedMessage, actualException.Message);
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
        public static async Task ShouldSafeGuardToAKnownMaxTemporaryPathLengthWhichStillWorksWithoutException()
        {
            string? veraPdfStartScript;

            var tempPath = Path.GetTempPath();

            using var pdfAValidatorMaxPath = new PdfAValidator();

            Assert.True(tempPath.Length < pdfAValidatorMaxPath.MaxLengthTempDirectoryThatVeraPdfFitsIn);

            while (tempPath.Length < pdfAValidatorMaxPath.MaxLengthTempDirectoryThatVeraPdfFitsIn)
            {
                tempPath += "X";
            }

            Directory.CreateDirectory(tempPath);

            try
            {
                using (var pdfAValidator = new PdfAValidator(tempPath))
                {
                    await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
                    veraPdfStartScript = pdfAValidator.VeraPdfStartScript;
                    Assert.True(File.Exists(veraPdfStartScript));
                }
                Assert.False(File.Exists(veraPdfStartScript));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public static async Task ShouldFailGracefullWithUnrecognicedVeraPdfOutput()
        {
            var somethingThatReturnsExitcode0 = "./TestExecuteables/exitcode0.bat";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                somethingThatReturnsExitcode0 = "TestExecuteables/exitcode0.sh";
            }

            var veraPdfException = await Assert.ThrowsAsync<VeraPdfException>(async () =>
              {
                  using var pdfAValidator = new PdfAValidator(somethingThatReturnsExitcode0, "SomeValue");
                  await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOfficeNonPdfA.pdf");
              });

            Assert.StartsWith($"Failed to parse VeraPdf Output: This is not XML", veraPdfException.Message);
            Assert.Contains($"Custom JAVACMD: SomeValue\nveraPdfStartScriptPath: {somethingThatReturnsExitcode0}", veraPdfException.Message);
        }

        [Fact]
        public static async Task ShouldFailGracefullWithExitcode2()
        {
            var somethingThatReturnsExitcode2 = "./TestExecuteables/exitcode2.bat";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                somethingThatReturnsExitcode2 = "TestExecuteables/exitcode2.sh";
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
                Assert.True(scriptPath?.EndsWith(".bat"));
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(scriptPath?.EndsWith("verapdf"));
            }
            Assert.True(File.Exists(scriptPath), scriptPath + " does not exist.");
        }

        [Fact]
        public static async Task ShouldNotFailOnMultipleDisposeCalls()
        {
            var postscriptValidator = new PdfAValidator();
            _ = await postscriptValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
            Assert.True(File.Exists(postscriptValidator.VeraPdfStartScript));
            postscriptValidator.Dispose();
            Assert.False(File.Exists(postscriptValidator.VeraPdfStartScript));
#pragma warning disable S3966 // Expecting multiple dispose calls to pass
            postscriptValidator.Dispose();
#pragma warning restore S3966 // Expecting multiple dispose calls to pass
            Assert.NotNull("Expectation is that this code does not throw.");
        }

        [Fact]
        public static void ShouldNotFailOnMultipleDisposeCallseWithoutInitialization()
        {
            var postscriptValidator = new PdfAValidator();

            postscriptValidator.Dispose();
#pragma warning disable S3966 // Expecting multiple dispose calls to pass
            postscriptValidator.Dispose();
#pragma warning restore S3966 // Expecting multiple dispose calls to pass

            Assert.NotNull("Expectation is that this code does not throw.");
        }
    }
}