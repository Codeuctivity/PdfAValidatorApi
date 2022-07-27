using Codeuctivity.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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
        private IVeraPdfOutputFilter? VeraPdfOutputFilter { get; }

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
        /// <param name="veraPdfOutputFilter">Optional VerapPdf console output filter</param>
        public PdfAValidator(string pathToVeraPdfBin, string pathToJava, IVeraPdfOutputFilter? veraPdfOutputFilter = null)
        {
            VeraPdfStartScript = pathToVeraPdfBin;
            PathJava = pathToJava;
            customVerapdfAndJavaLocations = true;
            IsInitialized = true;
            VeraPdfOutputFilter = veraPdfOutputFilter;
        }

        /// <summary>
        /// Use this constructor to use the embedded veraPdf binaries
        /// </summary>
        /// <param name="veraPdfOutputFilter">Optional VerapPdf console output filter</param>
        public PdfAValidator(IVeraPdfOutputFilter? veraPdfOutputFilter = null)
        {
            VeraPdfOutputFilter = veraPdfOutputFilter;
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
                    Arguments = $"{commandLineArguments} {string.Join(" ", pathArguments)}",
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            if (!string.IsNullOrEmpty(PathJava))
            {
                process.StartInfo.EnvironmentVariables["JAVACMD"] = PathJava;
            }

            WaitAndReceiveOutput(process, out var outputResult, out var errorResult);

            if (VeraPdfExitCodes.CanExitCodeBeParsed(process.ExitCode))
            {
                if (string.IsNullOrEmpty(outputResult))
                {
                    throw new VeraPdfException($"Calling VeraPdf exited with {process.ExitCode} without any output. Error: {errorResult}\nCustom JAVACMD: {PathJava}\nVeraPdfStartScript: {VeraPdfStartScript}");
                }

                var filterdResult = VeraPdfOutputFilter?.Filter(outputResult) ?? outputResult;

                ValidateVeraPdfOutputToBeXml(filterdResult, PathJava, VeraPdfStartScript);
                var veraPdfReport = DeserializeXml<Report>(filterdResult);
                veraPdfReport.RawOutput = outputResult;
                return veraPdfReport;
            }
            throw new VeraPdfException($"Calling VeraPdf exited with {process.ExitCode} caused an error: {errorResult}\nCustom JAVACMD: {PathJava}\nVeraPdfStartScript: {VeraPdfStartScript}");
        }

        private static void WaitAndReceiveOutput(Process process, out string outputResult, out string errorResult)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            using var outputWaitHandle = new AutoResetEvent(false);
            using var errorWaitHandle = new AutoResetEvent(false);
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            outputWaitHandle.WaitOne();
            errorWaitHandle.WaitOne();

            outputResult = outputBuilder.ToString();
            errorResult = errorBuilder.ToString();
        }

        private static bool IsSingleFolder(IEnumerable<string> pathsToPdfFiles)
        {
            var isSingle = pathsToPdfFiles.Count() == 1;
            if (isSingle)
            {
                var absolutePath = Path.GetFullPath(pathsToPdfFiles.First());
                return Directory.Exists(absolutePath);
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
                    var tasks = new List<Task>
                    {
                        ExtractBinaryFromManifest("Codeuctivity.Java.zip"),
                        // Downloaded from https://verapdf.org/software/ - verapdf-greenfield-1.20.3 - version seems to be out of sync compared to https://github.com/veraPDF/veraPDF-library/releases/ (latest there is v1.20.2)
                        ExtractBinaryFromManifest("Codeuctivity.VeraPdf.zip")
                    };

                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    VeraPdfStartScript = Path.Combine(pathVeraPdfDirectory, "verapdf", "verapdf.bat");
                    // took from https://adoptopenjdk.net/releases.html?variant=openjdk8&jvmVariant=hotspot#x64_win
                    PathJava = Path.Combine(pathVeraPdfDirectory, "verapdf", "jdk8u202-b08-jre", "bin", "java");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    await ExtractBinaryFromManifest("Codeuctivity.VeraPdf.zip").ConfigureAwait(false);
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
            var pathZipVeraPdf = Path.Combine(pathVeraPdfDirectory, $"{Guid.NewGuid()}.zip");
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