using Microsoft.AspNetCore.SignalR;

namespace LMDH_QS.Hubs
{
    public class QueueHub : Hub
    {
        public async Task BroadcastQueueUpdate()
        {
            await Clients.All.SendAsync("UpdateQueue");
        }
    }
}
