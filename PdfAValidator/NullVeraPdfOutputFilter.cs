namespace Codeuctivity
{
    /// <summary>
    /// Null output filter
    /// </summary>
    public class NullVeraPdfOutputFilter : IVeraPdfOutputFilter
    {
        /// <summary>
        /// Implement this to alter the console output of VeraPDF before it get parsed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Filter(string input)
        {
            return input;
        }
    }
}