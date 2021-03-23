using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeuctivity
{
    /// <summary>
    /// Vera PDF Exit codes that we handle
    /// </summary>
    public static class VeraPdfExitCodes
    {
        /// <summary>
        /// All files valid
        /// </summary>
        public static int AllFilesValid { get; }  = 0;

        /// <summary>
        /// Invalid PDF/A files found
        /// </summary>
        public static int InvalidPdfAFilesFound { get; } = 1;

        /// <summary>
        /// Failed to parse on or more files
        /// </summary>
        public static int FailedToParseOneOrMoreFiles { get; } = 7;

        /// <summary>
        /// Some PDFs encrypted
        /// </summary>
        public static int SomePdfsEncrypted { get; } = 8;

        /// <summary>
        /// Returns true if we can parse the output as xml
        /// </summary>
        public static bool CanExitCodeBeParsed(int exitCode)
        {
            return exitCode == AllFilesValid || exitCode == InvalidPdfAFilesFound || exitCode == FailedToParseOneOrMoreFiles || exitCode == SomePdfsEncrypted;
        }
    }
}
