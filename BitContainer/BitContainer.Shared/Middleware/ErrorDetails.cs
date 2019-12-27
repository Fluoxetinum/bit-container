using System;
using Newtonsoft.Json;

namespace BitContainer.Shared.Middleware
{
    public class ErrorDetails
    {
        public Int32 StatusCode { get; set; }
        public String Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
