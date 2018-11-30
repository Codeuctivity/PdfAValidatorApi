using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            Directory.Delete(_pathDirectoryVeraPdf, true);
        }

        /// <summary>
        /// Use this constructor to use your own installation of VeraPdf, e.g.: c:\somePath\verapdf.bat
        /// </summary>
        /// <param name="pathToVeraPdfBin"></param>
        public PdfAValidator(string pathToVeraPdfBin)
        { _pathVeraPdfBat = pathToVeraPdfBin; }

        /// <summary>
        /// Use this constructor to use the embedded veraPdf binaries
        /// </summary>
        public PdfAValidator()
        { _pathVeraPdfBat = getPathToVeraPdfBin(); }

        private string _pathDirectoryVeraPdf;
        private string _pathZipVeraPdf;
        private string _pathVeraPdfBat;

        public bool Validate(string pathToPdfFile)
        {
            return ValidateWithDetailedReport(pathToPdfFile).batchSummary.validationReports.compliant == 1;
        }

        public report ValidateWithDetailedReport(string pathToPdfFile)
        {
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

        /// <summary>
        /// Gets the path to VeraPdf bin.
        /// </summary>
        /// <returns></returns>
        private string getPathToVeraPdfBin()
        {
            _pathDirectoryVeraPdf = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid());
            Directory.CreateDirectory(_pathDirectoryVeraPdf);
            _pathZipVeraPdf = Path.Combine(_pathDirectoryVeraPdf, "VeraPdf.zip");
            using (FileStream fsDst = new FileStream(_pathZipVeraPdf, FileMode.CreateNew, FileAccess.Write))
            {
                byte[] bytes = Properties.Resources.VeraPdf;
                fsDst.Write(bytes, 0, bytes.Length);
            }
            ZipFile.ExtractToDirectory(_pathZipVeraPdf, _pathDirectoryVeraPdf);
            return Path.Combine(_pathDirectoryVeraPdf, "VeraPdf", "verapdf.bat");
        }
    }
}