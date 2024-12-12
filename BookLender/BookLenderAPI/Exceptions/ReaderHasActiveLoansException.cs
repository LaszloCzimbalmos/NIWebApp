using System;

namespace BookLenderAPI.Exceptions
{
    public class ReaderHasActiveLoansException : Exception
    {
        public ReaderHasActiveLoansException() : base("The reader has active loans and cannot be deleted.") { }

        public ReaderHasActiveLoansException(string message) : base(message) { }

        public ReaderHasActiveLoansException(string message, Exception innerException) : base(message, innerException) { }
    }
}
