using System;

namespace ArchPM.FluentRabbitMQ.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class FluentRabbitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentRabbitException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FluentRabbitException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentRabbitException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public FluentRabbitException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentRabbitException"/> class.
        /// </summary>
        public FluentRabbitException()
        {

        }
    }
}