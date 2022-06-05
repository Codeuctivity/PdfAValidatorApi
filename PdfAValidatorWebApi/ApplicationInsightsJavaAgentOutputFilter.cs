using Codeuctivity;

namespace PdfAValidatorWebApi
{
    /// <summary>
    /// Azure with enabled application insight adds some prefix before VeraPdf console output starts. This filter will remove the application insight specific output before it get parsed.
    /// </summary>
    public class ApplicationInsightsJavaAgentOutputFilter : IVeraPdfOutputFilter
    {
        /// <summary>
        /// Filter that gets applied on every console output of VeraPdf before it gets parsed.
        /// </summary>
        /// <param name="input">Unchanged console output of VeraPdf</param>
        /// <returns>Filtered console output of VeraPdf - omitting everything before first XML root node starts</returns>
        public string Filter(string input)
        {
            var startIndexVeraPdfOutput = input.IndexOf("<?XML", System.StringComparison.OrdinalIgnoreCase);

            if (startIndexVeraPdfOutput > 0)
            {
                return input[startIndexVeraPdfOutput..];
            }

            return input;
        }
    }
}