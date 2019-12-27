using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CSearchResult
    {
        public IAccessWrapper AccessWrapper { get; set; }
        public LinkedList<Guid> DownPath { get; set; }
    }
}
