namespace Codeuctivity
{
    internal class NullVeraPdfOutputFilter : IVeraPdfOutputFilter
    {
        public string Filter(string input)
        {
            return input;
        }
    }
}