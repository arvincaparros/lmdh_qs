using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMDH_QS.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext dbContext;

        public PublicController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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

        public IActionResult ConsultationQueueRows()
        {
            var queues = dbContext.Queues
                .Where(q => q.VisitDate.Date == DateTime.Today &&
                       (q.Status == "Consultation Standby" || q.Status == "Consultation Serving"))
                .Take(10)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return PartialView("_ConsultationQueueRows", queues); // or View()
        }

    }
}
