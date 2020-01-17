using System;

namespace BitContainer.Shared.Http.Exceptions
{
    public class UsernameExistsException : Exception
    {
        public UsernameExistsException(String message) : base(message) {}
    }
}
