using System;

namespace BitContainer.Contracts.V1.Shares
{
    public class CNewShareContract
    {
        public Guid EntityId { get; set; }
        public String UserName { get; set; } 
        public ERestrictedAccessTypeContract AccessTypeContract { get; set; }
        
        public static CNewShareContract Create(String userName, ERestrictedAccessTypeContract access, Guid entityId)
        {
            return new CNewShareContract()
            {
                UserName = userName,
                AccessTypeContract = access,
                EntityId = entityId
            };
        }
    }
}
