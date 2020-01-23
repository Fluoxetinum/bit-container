using System;
using BitContainer.Shared.Models;

namespace BitContainer.Contracts.V1.Auth
{
    public class CUserContract
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public CUserContract(Guid id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
