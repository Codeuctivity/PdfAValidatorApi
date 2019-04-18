using System;
using System.Runtime.Serialization;

namespace PdfAValidator
{
    [Serializable]
    internal class VeraPdfException : Exception
    {
        public VeraPdfException()
        {
        }

        public VeraPdfException(string message) : base(message)
        {
        }

        public VeraPdfException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VeraPdfException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}