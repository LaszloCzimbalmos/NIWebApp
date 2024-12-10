using System;

namespace BookLenderAPI.Exceptions
{
    public class IdMismatchException : NotSupportedException
    {
        public IdMismatchException() : base("Reader ID mismatch during update.") { }

        public IdMismatchException(string message) : base(message) { }

        public IdMismatchException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
