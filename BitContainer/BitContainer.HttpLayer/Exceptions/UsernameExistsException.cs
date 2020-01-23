using System;

namespace BitContainer.Http.Exceptions
{
    public class UsernameExistsException : Exception
    {
        public UsernameExistsException(String message) : base(message) {}
    }
}
