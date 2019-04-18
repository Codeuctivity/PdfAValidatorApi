using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

// TODOS:
// Performance - now every call causes verapdf to be unpacked
// Codestyle - make this code great again
// Fix the generated casing in report.cs

namespace PdfAValidator
{
    public class PdfAValidator : IDisposable
    {
        public void Dispose()
        {
            Directory.Delete(_pathVeraPdfDirectory, true);
        }

        /// <summary>
        /// Use this constructor to use your own installation of VeraPdf and Java, e.g.: c:\somePath\verapdf.bat
        /// </summary>
        /// <param name="pathToVeraPdfBin"></param>
        public PdfAValidator(string pathToVeraPdfBin, string pathToJava)
        {
            // TODO Test
            VeraPdfStarterScript = pathToVeraPdfBin;
            PathJava = pathToJava;
        }

        /// <summary>
        /// Use this constructor to use the embedded veraPdf binaries
        /// </summary>
        public PdfAValidator()
        { intiPathToVeraPdfBinAndJava(); }

        private string _pathVeraPdfDirectory;
        public string PathJava { private set; get; }
        private string _pathZipVeraPdf;
        public string VeraPdfStarterScript { private set; get; }

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

                process.StartInfo.FileName = VeraPdfStarterScript;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                if (!String.IsNullOrEmpty(PathJava))
                    process.StartInfo.EnvironmentVariables["JAVACMD"] = PathJava;
                ProcessStartInfo startInfo = process.StartInfo;
                string[] arguments = new string[] { "\"", pathToPdfFile, "\" " };
                startInfo.Arguments = string.Concat(arguments);
                process.Start();

                var outputResult = GetStreamOutput(process.StandardOutput);
                var errorResult = GetStreamOutput(process.StandardError);

                process.WaitForExit();

                if (string.IsNullOrEmpty(errorResult))
                {
                    var veraPdfReport = DeserializeXml<report>(outputResult);
                    return veraPdfReport;
                }

                throw new VeraPdfException("Calling VearPdf caused an error: " + errorResult);
            }
        }

        private string GetStreamOutput(StreamReader stream)
        {
            //Read output in separate task to avoid deadlocks
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }

        static T DeserializeXml<T>(string sourceXML) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            T result = null;

            using (TextReader reader = new StringReader(sourceXML))
                result = (T)serializer.Deserialize(reader);
            return result;
        }

        public void SetLinuxFileExecuteable(string filePath)
        {
            var chmodCmd = "chmod 700 " + filePath;
            var escapedArgs = chmodCmd.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\""
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private void intiPathToVeraPdfBinAndJava()
        {
            _pathVeraPdfDirectory = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid());
            Directory.CreateDirectory(_pathVeraPdfDirectory);
            _pathZipVeraPdf = Path.Combine(_pathVeraPdfDirectory, "VeraPdf.zip");

            var assembly = Assembly.GetExecutingAssembly();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var stream = assembly.GetManifestResourceStream("PdfAValidator.VeraPdf.Windows.zip"))
                using (var fileStream = File.Create(_pathZipVeraPdf))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
                ZipFile.ExtractToDirectory(_pathZipVeraPdf, _pathVeraPdfDirectory);
                VeraPdfStarterScript = Path.Combine(_pathVeraPdfDirectory, "verapdf", "verapdf.bat");
                // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
                PathJava = Path.Combine(_pathVeraPdfDirectory, "verapdf", "jdk8u202-b08-jre", "bin", "java");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var names = assembly.GetManifestResourceNames();

                using (var stream = assembly.GetManifestResourceStream("PdfAValidator.VeraPdf.Linux.zip"))
                using (var fileStream = File.Create(_pathZipVeraPdf))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
                ZipFile.ExtractToDirectory(_pathZipVeraPdf, _pathVeraPdfDirectory);

                VeraPdfStarterScript = Path.Combine(_pathVeraPdfDirectory, "verapdf", "verapdf");
                SetLinuxFileExecuteable(VeraPdfStarterScript);
            }
            else
                throw new NotImplementedException("Sorry, only supporting linux and windows.");
        }
    }
}