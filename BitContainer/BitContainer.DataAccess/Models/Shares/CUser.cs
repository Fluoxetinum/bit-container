using System;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models.Shares
{
    public class CUser
    {
        public CUserId Id { get; set; }
        public String Name { get; set; }

        public CUser (CUserId id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
