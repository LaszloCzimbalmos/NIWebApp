using System;

namespace BookLenderAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Record not found.") { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
