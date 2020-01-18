using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Shares
{
    public class CSearchResultContract
    {
        public CAccessWrapperContract AccessWrapper { get; set; }
        public LinkedList<Guid> DownPath { get; set; }
    }
}
