using LMDH_QS.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LMDH_QS.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IHubContext<QueueHub> _hubContext;

        public PublicController(AppDbContext dbContext, IHubContext<QueueHub> hubContext)
        {
            this.dbContext = dbContext;
            this._hubContext = hubContext;
        }

        public IActionResult PublicDisplay()
        {
            var today = DateTime.Today;
            var queue = dbContext.Queues
                .Where(q => q.VisitDate == today)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return View(queue);
        }

        public IActionResult QueueRows()
        {
            var queues = dbContext.Queues
                .Where(q => q.VisitDate.Date == DateTime.Today &&
                       (q.Status == "Standby" || q.Status == "Serving"))
                .Take(10)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return PartialView("_QueueRows", queues); // or View()
        }

        public async Task<IActionResult> ConsultationQueueRows()
        {
            var queues = dbContext.Queues
                .Where(q => q.VisitDate.Date == DateTime.Today &&
                       (q.Status == "Consultation Standby" || q.Status == "Consultation Serving"))
                .Take(10)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            await _hubContext.Clients.All.SendAsync("UpdateQueue");
            await _hubContext.Clients.All.SendAsync("UpdateConsultationQueue");

            return PartialView("_ConsultationQueueRows", queues); // or View()
        }

    }
}
