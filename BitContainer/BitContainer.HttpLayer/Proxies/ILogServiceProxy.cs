using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;

namespace BitContainer.Http.Proxies
{
    public interface ILogServiceProxy
    {
        Task LogRequest(CNlogMessageContract message);
    }
}
