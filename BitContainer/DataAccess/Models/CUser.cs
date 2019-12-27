using System;

namespace BitContainer.DataAccess.Models
{
    public class CUser
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public static CUser Create(Guid id, String name)
        {
            return new CUser()
            {
                Id = id,
                Name = name
            };
        }
    }
}
