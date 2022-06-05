namespace Codeuctivity
{
    /// <summary>
    /// IVeraPdfOutputFilter
    /// </summary>
    public interface IVeraPdfOutputFilter
    {
        /// <summary>
        /// Filter that gets applied on every console output of VeraPdf before it gets parsed.
        /// </summary>
        /// <param name="input">Unchanged console output of VeraPdf</param>
        /// <returns>Filtered console output of VeraPdf</returns>
        string Filter(string input);
    }
}