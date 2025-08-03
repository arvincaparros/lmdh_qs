using Microsoft.AspNetCore.SignalR;

namespace LMDH_QS.Hubs
{
    public class QueueHub : Hub
    {
        public async Task BroadcastQueueUpdate()
        {
            await Clients.All.SendAsync("UpdateQueue");
        }

        // 🔄 Specific to Consultation Table
        public async Task UpdateConsultation()
        {
            await Clients.All.SendAsync("UpdateConsultationQueue");
        }

        // 🔄 Specific to Pre-Assessment Table
        public async Task UpdatePreAssessment()
        {
            await Clients.All.SendAsync("UpdatePreAssessmentQueue");
        }

        // 🔄 Optional: Trigger all tables (e.g., from admin action)
        public async Task UpdateAll()
        {
            await Clients.All.SendAsync("UpdateQueue");
            await Clients.All.SendAsync("UpdateConsultationQueue");
            await Clients.All.SendAsync("UpdatePreAssessmentQueue");
        }

    }
}
