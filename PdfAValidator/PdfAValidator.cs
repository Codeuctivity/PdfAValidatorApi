using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PdfAValidator
{
    /// <summary>
    /// PdfAValidator is a VeraPdf wrapper
    /// </summary>
    public class PdfAValidator : IDisposable
    {
        private const string maskedQuote = "\"";
        private const int maxLengthTempdirectoryThatVeraPdfFitsIn = 206;
        private const string OsNotSupportedMessage = "Sorry, only supporting linux and windows.";
        private readonly object lockObject = new object();
        private string? pathVeraPdfDirectory;
        private bool disposed;

        /// <summary>
        /// Path to java jre used by windows
        /// </summary>
        /// <value></value>
        public string? PathJava { private set; get; }

        private readonly bool customVerapdfAndJavaLocations;

        private bool IsInitialized { get; set; }

        /// <summary>
        /// Command that is used to invoke VeraPdf
        /// </summary>
        /// <value>Command with arguments</value>
        public string? VeraPdfStartScript { private set; get; }

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
                if (!customVerapdfAndJavaLocations && Directory.Exists(pathVeraPdfDirectory))
                {
                    Directory.Delete(pathVeraPdfDirectory, true);
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
            customVerapdfAndJavaLocations = true;
            IsInitialized = true;
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
            return ValidateWithDetailedReport(pathToPdfFile).BatchSummary.ValidationReports.Compliant == "1";
        }

        /// <summary>
        /// Validates a pdf and returns a detailed compliance report
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns></returns>
        public Report ValidateWithDetailedReport(string pathToPdfFile)
        {
            IntiPathToVeraPdfBinAndJava();
            var absolutePathToPdfFile = Path.GetFullPath(pathToPdfFile);

            if (!File.Exists(absolutePathToPdfFile))
            {
                throw new FileNotFoundException(absolutePathToPdfFile + " not found");
            }

            using var process = new Process();
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

            if (process.ExitCode == 0)
            {
                validateVeraPdfOutputToBeXml(outputResult, PathJava, VeraPdfStartScript);
                var veraPdfReport = DeserializeXml<Report>(outputResult);
                return veraPdfReport;
            }
            throw new VeraPdfException($"Calling VeraPdf exited with {process.ExitCode} caused an error: {errorResult}\nCustom JAVACMD: {PathJava}\nVeraPdfStartScript: {VeraPdfStartScript}");
        }

        private void validateVeraPdfOutputToBeXml(string outputResult, string? customJavaCmd, string? veraPdfStartScript)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(outputResult);
            }
            catch (XmlException xmlException)
            {
                throw new VeraPdfException($"Failed to parse VeraPdf Ouput: {outputResult}\nCustom JAVACMD: {customJavaCmd}\nveraPdfStartScriptPath: {veraPdfStartScript}", xmlException);
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
            var settings = new XmlReaderSettings();
            var serializer = new XmlSerializer(typeof(T));

            using var reader = new StringReader(sourceXML);
            using var xmlReader = XmlReader.Create(reader, settings);
            return (T)serializer.Deserialize(xmlReader);
        }

        private void IntiPathToVeraPdfBinAndJava()
        {
            lock (lockObject)
            {
                if (IsInitialized)
                {
                    return;
                }

                pathVeraPdfDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                if (pathVeraPdfDirectory.Length > maxLengthTempdirectoryThatVeraPdfFitsIn)
                {
                    throw new PathTooLongException(pathVeraPdfDirectory);
                }

                Directory.CreateDirectory(pathVeraPdfDirectory);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ExtractBinaryFromManifest("PdfAValidator.VeraPdf.Windows.zip");
                    VeraPdfStartScript = Path.Combine(pathVeraPdfDirectory, "verapdf", "verapdf.bat");
                    // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
                    PathJava = Path.Combine(pathVeraPdfDirectory, "verapdf", "jdk8u202-b08-jre", "bin", "java");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ExtractBinaryFromManifest("PdfAValidator.VeraPdf.Linux.zip");
                    VeraPdfStartScript = Path.Combine(pathVeraPdfDirectory, "verapdf", "verapdf");
                    SetLinuxFileExecuteable(VeraPdfStartScript);
                }
                else
                {
                    throw new NotImplementedException(OsNotSupportedMessage);
                }

                IsInitialized = true;
            }
        }

        private static void SetLinuxFileExecuteable(string filePath)
        {
            var chmodCmd = "chmod 700 " + filePath;
            var escapedArgs = chmodCmd.Replace(maskedQuote, "\\\"");

            using var process = new Process
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

        private void ExtractBinaryFromManifest(string resourceName)
        {
            var pathZipVeraPdf = Path.Combine(pathVeraPdfDirectory, "VeraPdf.zip");
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var fileStream = File.Create(pathZipVeraPdf))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            ZipFile.ExtractToDirectory(pathZipVeraPdf, pathVeraPdfDirectory);
        }
    }
}