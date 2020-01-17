using System;

namespace BitContainer.Shared.Http.Exceptions
{
    public class NoSuchUserException : Exception
    {
        public NoSuchUserException(String message) : base(message) {}
    }
}
