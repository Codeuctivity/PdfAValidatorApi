using System;
using System.Threading.Tasks;

namespace Codeuctivity
{
    /// <summary>
    /// PdfAValidator is a VeraPdf wrapper
    /// </summary>
    public interface IPdfAValidator : IDisposable
    {
        /// <summary>
        /// Validates a pdf to be compliant with the pdfa standard claimed by its meta data
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <returns>True for compliant PdfA Files</returns>
        Task<bool> ValidateAsync(string pathToPdfFile);

        /// <summary>
        /// Validates a pdf and returns a detailed compliance report
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <param name="otherArguments">Command line arguments</param>
        /// <returns></returns>
        Task<Report> ValidateWithDetailedReportAsync(string pathToPdfFile, string otherArguments = "");
    }
}