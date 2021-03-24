using Codeuctivity.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Codeuctivity
{
    /// <summary>
    /// PdfAValidator is a VeraPdf wrapper
    /// </summary>

    public class PdfAValidator : IPdfAValidator
    {
        private const string maskedQuote = "\"";
        private const int maxLengthTempdirectoryThatVeraPdfFitsIn = 206;
        private string? pathVeraPdfDirectory;
        private bool disposed;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
        /// Disposing VeraPdf bins
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing VeraPdf bins
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing && !customVerapdfAndJavaLocations && Directory.Exists(pathVeraPdfDirectory))
            {
                Directory.Delete(pathVeraPdfDirectory, true);
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
        {
        }

        /// <summary>
        /// Validates a pdf to be compliant with the pdfa standard claimed by its meta data
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns>True for compliant PdfA Files</returns>
        public async Task<bool> ValidateAsync(string pathToPdfFile)
        {
            return (await ValidateWithDetailedReportAsync(pathToPdfFile).ConfigureAwait(false)).BatchSummary.ValidationReports.Compliant == "1";
        }

        /// <summary>
        /// Validates a pdf and returns a detailed compliance report
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns></returns>
        public async Task<Report> ValidateWithDetailedReportAsync(string pathToPdfFile)
        {
            return await ValidateWithDetailedReportAsync(pathToPdfFile, "").ConfigureAwait(false);
        }

        /// <summary>
        /// Validates a pdf and returns a detailed compliance report
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <param name="commandLineArguments">Command line arguments</param>
        /// <returns></returns>
        public async Task<Report> ValidateWithDetailedReportAsync(string pathToPdfFile, string commandLineArguments)
        {
            return await ValidateBatchWithDetailedReportAsync(new[] { pathToPdfFile }, commandLineArguments).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates a batch of pdf files and returns a detailed compliance report
        /// </summary>
        /// <param name="pathsToPdfFiles"></param>
        /// <param name="commandLineArguments">Command line arguments</param>
        /// <returns></returns>
        public async Task<Report> ValidateBatchWithDetailedReportAsync(IEnumerable<string> pathsToPdfFiles, string commandLineArguments)
        {
            await IntiPathToVeraPdfBinAndJava().ConfigureAwait(false);

            if (!IsSingleFolder(pathsToPdfFiles))
            {
                ValidatePdfFilesExist(pathsToPdfFiles);
            }
            // http://docs.verapdf.org/cli/terminal/
            var pathArguments = pathsToPdfFiles.Select(path => @$"""{Path.GetFullPath(path)}""");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = VeraPdfStartScript,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"{commandLineArguments} {string.Join(" ", pathArguments)}"
                }
            };

            if (!string.IsNullOrEmpty(PathJava))
            {
                process.StartInfo.EnvironmentVariables["JAVACMD"] = PathJava;
            }

            process.Start();

            var outputResult = GetStreamOutput(process.StandardOutput);
            var errorResult = GetStreamOutput(process.StandardError);

            process.WaitForExit();

            if (VeraPdfExitCodes.CanExitCodeBeParsed(process.ExitCode))
            {
                if (string.IsNullOrEmpty(outputResult))
                {
                    throw new VeraPdfException($"Calling VeraPdf exited with {process.ExitCode} without any output. Error: {errorResult}\nCustom JAVACMD: {PathJava}\nVeraPdfStartScript: {VeraPdfStartScript}");
                }
                ValidateVeraPdfOutputToBeXml(outputResult, PathJava, VeraPdfStartScript);
                var veraPdfReport = DeserializeXml<Report>(outputResult);
                return veraPdfReport;
            }
            throw new VeraPdfException($"Calling VeraPdf exited with {process.ExitCode} caused an error: {errorResult}\nCustom JAVACMD: {PathJava}\nVeraPdfStartScript: {VeraPdfStartScript}");
        }

        private bool IsSingleFolder(IEnumerable<string> pathsToPdfFiles)
        {
            bool isSingle = pathsToPdfFiles.Count() == 1;
            if (isSingle)
            {
                var absolutePath = Path.GetFullPath(pathsToPdfFiles.First());
                if (Directory.Exists(absolutePath))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ValidatePdfFilesExist(IEnumerable<string> pathsToPdfFiles)
        {
            foreach (var pathToPdfFile in pathsToPdfFiles)
            {
                var absolutePathToPdfFile = Path.GetFullPath(pathToPdfFile);

                if (!File.Exists(absolutePathToPdfFile))
                {
                    throw new FileNotFoundException(absolutePathToPdfFile + " not found");
                }
            }
        }

        private static void ValidateVeraPdfOutputToBeXml(string outputResult, string? customJavaCmd, string? veraPdfStartScript)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(outputResult);
            }
            catch (XmlException xmlException)
            {
                throw new VeraPdfException($"Failed to parse VeraPdf Output: {outputResult}\nCustom JAVACMD: {customJavaCmd}\nveraPdfStartScriptPath: {veraPdfStartScript}", xmlException);
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

        private async Task IntiPathToVeraPdfBinAndJava()
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
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
                    await ExtractBinaryFromManifest("Codeuctivity.VeraPdf.Windows.zip").ConfigureAwait(false);
                    VeraPdfStartScript = Path.Combine(pathVeraPdfDirectory, "verapdf", "verapdf.bat");
                    // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
                    PathJava = Path.Combine(pathVeraPdfDirectory, "verapdf", "jdk8u202-b08-jre", "bin", "java");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    await ExtractBinaryFromManifest("Codeuctivity.VeraPdf.Linux.zip").ConfigureAwait(false);
                    VeraPdfStartScript = Path.Combine(pathVeraPdfDirectory, "verapdf", "verapdf");
                    SetLinuxFileExecuteable(VeraPdfStartScript);
                }
                else
                {
                    throw new NotImplementedException(Resources.OsNotSupportedMessage);
                }

                IsInitialized = true;
            }
            finally
            {
                semaphore.Release();
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

        private async Task ExtractBinaryFromManifest(string resourceName)
        {
            var pathZipVeraPdf = Path.Combine(pathVeraPdfDirectory, "VeraPdf.zip");
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var fileStream = File.Create(pathZipVeraPdf))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            ZipFile.ExtractToDirectory(pathZipVeraPdf, pathVeraPdfDirectory);
        }
    }
}