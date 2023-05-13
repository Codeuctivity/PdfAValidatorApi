using Codeuctivity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace PdfAValidatorWebApi.Controllers
{
    /// <summary>
    /// <see cref="PdfAValidatorController"/>
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PdfAValidatorController : ControllerBase
    {
        private IPdfAValidator PdfAValidator { get; }

        /// <summary>
        /// Inject the validator.
        /// </summary>
        public PdfAValidatorController(IPdfAValidator PdfAValidator)
        {
            this.PdfAValidator = PdfAValidator;
        }

        /// <summary>
        /// Validates the compliance of a PdfA.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns>Compliance</returns>
        /// <response code="200">Returns the result</response>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<ActionResult> Validate([Required] IFormFile pdfFile)
        {
            var uploadedFile = pdfFile.OpenReadStream();
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid() + ".pdf");
            try
            {
                using var fs = new FileStream(tempPdfFilePath, FileMode.CreateNew, FileAccess.Write);
                await uploadedFile.CopyToAsync(fs);

                var result = await PdfAValidator.ValidateAsync(tempPdfFilePath);
                return Ok(result);
            }
            finally
            {
                if (System.IO.File.Exists(tempPdfFilePath))
                {
                    System.IO.File.Delete(tempPdfFilePath);
                }
            }
        }

        /// <summary>
        /// Validates the compliance of a PDF(A) and gives some validation detail.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns>Compliance</returns>
        /// <response code="200">Returns a report about the analyzed PDF, e.g. PdfA substandard and compliance violations</response>
        [HttpPost]
        [Route("DetailedReport")]
        [ProducesResponseType(200, Type = typeof(Report))]
        public async Task<Report> ValidateWithDetailedReport([Required] IFormFile pdfFile)
        {
            using var uploadedFile = pdfFile.OpenReadStream();
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid() + ".pdf");
            try
            {
                using var fs = new FileStream(tempPdfFilePath, FileMode.CreateNew, FileAccess.Write);
                await uploadedFile.CopyToAsync(fs);

                var result = await PdfAValidator.ValidateWithDetailedReportAsync(tempPdfFilePath);
                return result;
            }
            finally
            {
                if (System.IO.File.Exists(tempPdfFilePath))
                {
                    System.IO.File.Delete(tempPdfFilePath);
                }
            }
        }
    }
}