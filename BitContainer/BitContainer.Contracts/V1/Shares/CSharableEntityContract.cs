using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Shares
{
    public class CSharableEntityContract
    {
        public CAccessWrapperContract AccessWrapper { get; set; }
        public List<CShareContract> Shares { get; set; }

        public CSharableEntityContract(CAccessWrapperContract accessWrapper, List<CShareContract> shares)
        {
            AccessWrapper = accessWrapper;
            Shares = shares;
        }

    }
}
