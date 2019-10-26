using System;

namespace ArchPM.FluentRabbitMQ.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ModelIsNullException : FluentRabbitException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelIsNullException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModelIsNullException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelIsNullException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ModelIsNullException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelIsNullException"/> class.
        /// </summary>
        public ModelIsNullException() : this("Model is null. Connect method must be called first!")
        {

        }
    }
}