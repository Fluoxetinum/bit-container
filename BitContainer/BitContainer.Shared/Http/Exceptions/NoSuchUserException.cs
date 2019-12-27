using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitContainer.Presentation.Controllers.Proxies.Exceptions
{
    public class NoSuchUserException : Exception
    {
        public NoSuchUserException(String message) : base(message) {}
    }
}
