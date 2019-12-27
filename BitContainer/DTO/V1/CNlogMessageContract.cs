using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BitContainer.LogService.Models
{
    public class CNlogMessageContract
    {
        public String Message { get; set; }
        public String NLogLevel { get; set; }
        public String Exception { get; set; }
    }
}
