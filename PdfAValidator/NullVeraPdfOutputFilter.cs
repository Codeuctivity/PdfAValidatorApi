namespace Codeuctivity
{
    internal class NullVeraPdfOutputFilter : IVeraPdfOutputFilter
    {
        public NullVeraPdfOutputFilter()
        {
        }

        public string Filter(string input)
        {
            return input;
        }
    }
}