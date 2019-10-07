using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

// TODO Fix the generated casing in report.cs

namespace PdfAValidator
{
    /// <summary>
    /// PdfAValidator is a VeraPdf wrapper
    /// </summary>
    public class PdfAValidator : IDisposable
    {
        private const string maskedQuote = "\"";
        private string _pathVeraPdfDirectory;
        private bool disposed;

        /// <summary>
        /// Path to java jre used by windows
        /// </summary>
        /// <value></value>
        public string PathJava { private set; get; }

        private readonly bool _customVerapdfAndJavaLocations;

        private bool IsInitilized { get; set; }

        /// <summary>
        /// Command that is used to invoke VeraPdf
        /// </summary>
        /// <value>Command with arguments</value>
        public string VeraPdfStartScript { private set; get; }

        /// <summary>
        /// Disposing verapdf bins
        /// </summary>

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing ghostscript bins
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!_customVerapdfAndJavaLocations && Directory.Exists(_pathVeraPdfDirectory))
                {
                    Directory.Delete(_pathVeraPdfDirectory, true);
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Use this constructor to use your own installation of VeraPdf and Java, e.g.: c:\somePath\verapdf.bat
        /// </summary>
        /// <param name="pathToVeraPdfBin"></param>
        /// <param name="pathToJava"></param>
        public PdfAValidator(string pathToVeraPdfBin, string pathToJava)
        {
            VeraPdfStartScript = pathToVeraPdfBin;
            PathJava = pathToJava;
            _customVerapdfAndJavaLocations = true;
            IsInitilized = true;
        }

        /// <summary>
        /// Use this constructor to use the embedded veraPdf binaries
        /// </summary>
        public PdfAValidator()
        { }

        /// <summary>
        /// Validates a pdf to be compliant with the pdfa standard claimed by its meta data
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns>True for compliant PdfA Files</returns>
        public bool Validate(string pathToPdfFile)
        {
            return ValidateWithDetailedReport(pathToPdfFile).batchSummary.validationReports.compliant == "1";
        }

        /// <summary>
        /// Validates a pdf and returns a detailed compliance report
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns></returns>
        public report ValidateWithDetailedReport(string pathToPdfFile)
        {
            IntiPathToVeraPdfBinAndJava();
            var absolutePathToPdfFile = Path.GetFullPath(pathToPdfFile);

            if (!File.Exists(absolutePathToPdfFile))
            {
                throw new FileNotFoundException(absolutePathToPdfFile + " not found");
            }

            using (var process = new Process())
            {
                process.StartInfo.FileName = VeraPdfStartScript;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                if (!string.IsNullOrEmpty(PathJava))
                {
                    process.StartInfo.EnvironmentVariables["JAVACMD"] = PathJava;
                }
                var startInfo = process.StartInfo;
                // http://docs.verapdf.org/cli/terminal/
                var arguments = new[] { maskedQuote, absolutePathToPdfFile, maskedQuote };
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

        private static string GetStreamOutput(StreamReader stream)
        {
            //Read output in separate task to avoid deadlocks
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }

        private static T DeserializeXml<T>(string sourceXML) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            T result = null;

            using (TextReader reader = new StringReader(sourceXML))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        private void IntiPathToVeraPdfBinAndJava()
        {
            if (IsInitilized)
            {
                return;
            }

            _pathVeraPdfDirectory = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid());
            Directory.CreateDirectory(_pathVeraPdfDirectory);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ExtractBinaryFromManifest("PdfAValidator.VeraPdf.Windows.zip");
                VeraPdfStartScript = Path.Combine(_pathVeraPdfDirectory, "verapdf", "verapdf.bat");
                // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
                PathJava = Path.Combine(_pathVeraPdfDirectory, "verapdf", "jdk8u202-b08-jre", "bin", "java");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ExtractBinaryFromManifest("PdfAValidator.VeraPdf.Linux.zip");
                VeraPdfStartScript = Path.Combine(_pathVeraPdfDirectory, "verapdf", "verapdf");
                SetLinuxFileExecuteable(VeraPdfStartScript);
            }
            else
            {
                throw new NotImplementedException("Sorry, only supporting linux and windows.");
            }
            IsInitilized = true;
        }

        private static void SetLinuxFileExecuteable(string filePath)
        {
            var chmodCmd = "chmod 700 " + filePath;
            var escapedArgs = chmodCmd.Replace(maskedQuote, "\\\"");

            using (var process = new Process
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
            })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        private void ExtractBinaryFromManifest(string resourceName)
        {
            var pathZipVeraPdf = Path.Combine(_pathVeraPdfDirectory, "VeraPdf.zip");
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var fileStream = File.Create(pathZipVeraPdf))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
            ZipFile.ExtractToDirectory(pathZipVeraPdf, _pathVeraPdfDirectory);
        }
    }
}