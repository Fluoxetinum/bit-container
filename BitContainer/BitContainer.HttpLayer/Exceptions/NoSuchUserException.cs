using System;

namespace BitContainer.Http.Exceptions
{
    public class NoSuchUserException : Exception
    {
        public NoSuchUserException(String message) : base(message) {}
    }
}
