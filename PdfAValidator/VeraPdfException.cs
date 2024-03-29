using System;

namespace Codeuctivity
{
    /// <summary>
    /// The exception that is thrown when the call of verapdf fails execution of a program.
    /// </summary>
    public class VeraPdfException : Exception
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public VeraPdfException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the PdfAValidator.VeraPdfException class with a specified
        ///     error message exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public VeraPdfException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the PdfAValidator.VeraPdfException class with a specified
        ///     error message and a reference to the inner exception that is the cause of this
        ///     exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public VeraPdfException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}