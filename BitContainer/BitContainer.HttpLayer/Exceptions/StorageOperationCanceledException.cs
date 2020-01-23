using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Http.Exceptions
{
    public class StorageOperationCanceledException : Exception
    {
        public StorageOperationCanceledException(string message):base(message) {}
    }
}
