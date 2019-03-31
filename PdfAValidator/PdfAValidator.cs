using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;

// TODOS:
// Performance - now every call causes verapdf to be unpacked
// Inportability - Make verapdf working on linux
// Codestyle - make this code great again
// Fix the generated casing in report.cs

namespace PdfAValidator
{
    public class PdfAValidator : IDisposable
    {
        public void Dispose()
        {
            Directory.Delete(_pathVeraPdf, true);
        }

        /// <summary>
        /// Use this constructor to use your own installation of VeraPdf and Java, e.g.: c:\somePath\verapdf.bat
        /// </summary>
        /// <param name="pathToVeraPdfBin"></param>
        public PdfAValidator(string pathToVeraPdfBin, string pathToJava)
        {
            _pathVeraPdfBat = pathToVeraPdfBin;
            _pathJava = pathToJava;
        }

        /// <summary>
        /// Use this constructor to use the embedded veraPdf binaries
        /// </summary>
        public PdfAValidator()
        { intiPathToVeraPdfBinAndJava(); }

        private string _pathVeraPdf;
        private string _pathJava;
        private string _pathZipVeraPdf;
        private string _pathVeraPdfBat;

        public bool Validate(string pathToPdfFile)
        {
            return ValidateWithDetailedReport(pathToPdfFile).batchSummary.validationReports.compliant == 1;
        }

        public report ValidateWithDetailedReport(string pathToPdfFile)
        {
            pathToPdfFile = System.IO.Path.GetFullPath(pathToPdfFile);

            if (!File.Exists(pathToPdfFile))
            {
                throw new FileNotFoundException(pathToPdfFile + " not found");
            }

            using (var process = new Process())
            {
                string concatedVeraPdfOutput = string.Empty;

                process.StartInfo.FileName = _pathVeraPdfBat;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.EnvironmentVariables["JAVACMD"] = _pathJava;
                ProcessStartInfo startInfo = process.StartInfo;
                string[] arguments = new string[] { "\"", pathToPdfFile, "\" " };
                startInfo.Arguments = string.Concat(arguments);
                process.Start();

                while (true)
                {
                    string standardOutputLine = process.StandardOutput.ReadLine();
                    if (standardOutputLine == null)
                    {
                        using (var sr = new StringReader(concatedVeraPdfOutput))
                        {
                            var veraPdfReport = (report)new XmlSerializer(typeof(report)).Deserialize(sr);
                            return veraPdfReport;
                        }
                    }

                    concatedVeraPdfOutput += standardOutputLine;
                }
            }
        }

        private void intiPathToVeraPdfBinAndJava()
        {
            _pathVeraPdf = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid());
            Directory.CreateDirectory(_pathVeraPdf);
            _pathZipVeraPdf = Path.Combine(_pathVeraPdf, "VeraPdf.zip");

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("PdfAValidator.VeraPdf.zip"))
            using (var fileStream = File.Create(_pathZipVeraPdf))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            ZipFile.ExtractToDirectory(_pathZipVeraPdf, _pathVeraPdf);
            _pathVeraPdfBat = Path.Combine(_pathVeraPdf, "VeraPdf", "verapdf.bat");
            // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
            _pathJava = Path.Combine(_pathVeraPdf, "VeraPdf", "jdk8u202-b08-jre", "bin", "java");
        }
    }
}