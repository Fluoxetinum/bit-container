using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models.Shares
{
    public class CSharableEntity
    {
        public CAccessWrapper AccessWrapper { get; set; }
        public List<CShare> Shares { get; set; }

        public CSharableEntity(CAccessWrapper accessWrapper, List<CShare> shares)
        {
            AccessWrapper = accessWrapper;
            Shares = shares;
        }
    }
}
