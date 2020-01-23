using System;
using System.Threading.Tasks;
using BitContainer.Service.Storage.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BitContainer.Service.Storage.Managers
{
    [Authorize]
    public class CEventsHub : Hub<IEventsHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
