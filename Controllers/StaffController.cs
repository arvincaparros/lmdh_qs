using LMDH_QS.Hubs;
using LMDH_QS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LMDH_QS.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<QueueHub> _hubContext;

        public StaffController(AppDbContext dbContext, IHubContext<QueueHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public IActionResult StaffControl()
        {
            var todayQueues = _dbContext.Queues
                .Where(q => q.VisitDate.Date == DateTime.Today)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return View(todayQueues);
        }

        public IActionResult QueueRows()
        {
            var today = DateTime.Today;
            var queues = _dbContext.Queues
                .Where(q => q.VisitDate.Date == today &&
                       (q.Status == "Standby" || q.Status == "Serving"))
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return PartialView("_QueueRows", queues);
        }


        //[HttpPost]
        //public async Task<IActionResult> Call([FromBody] QueueActionRequest request)
        //{
        //    var queue = _dbContext.Queues.FirstOrDefault(q =>
        //        q.PatientIdentification == request.PatientIdentification &&
        //        q.VisitDate.Date == request.VisitDate.Date &&
        //        q.Department == request.Department);

        //    if (queue != null)
        //    {
        //        // 👇 Just notify clients, no status update
        //        await _hubContext.Clients.All.SendAsync("CallPatient", new
        //        {
        //            queue.PatientName,
        //            TicketNo = $"{queue.DeptCode}{queue.QueueNumber:D3}",
        //            queue.Department
        //        });

        //        return Json(new { success = true });
        //    }

        //    return Json(new { success = false });
        //}

        //[HttpPost]
        //public async Task<IActionResult> Call([FromBody] QueueActionRequest request)
        //{
        //    var queue = _dbContext.Queues.FirstOrDefault(q =>
        //        q.PatientIdentification == request.PatientIdentification &&
        //        q.VisitDate.Date == request.VisitDate.Date &&
        //        q.Department == request.Department);

        //    if (queue != null)
        //    {
        //        string currentTicket = $"{queue.DeptCode}{queue.QueueNumber:D3}";

        //        // 🔍 Find the next standby ticket for the same department
        //        var next = _dbContext.Queues
        //            .Where(q =>
        //                q.Department == request.Department &&
        //                q.VisitDate.Date == DateTime.Today &&
        //                q.Status == "Standby" &&
        //                q.QueueNumber > queue.QueueNumber)
        //            .OrderBy(q => q.QueueNumber)
        //            .FirstOrDefault();

        //        string? nextTicket = next != null
        //            ? $"{next.DeptCode}{next.QueueNumber:D3}"
        //            : null;

        //        // 🔊 Broadcast full call info
        //        await _hubContext.Clients.All.SendAsync("CallPatient", new
        //        {
        //            PatientName = queue.PatientName,
        //            TicketNo = currentTicket,
        //            Department = queue.Department,
        //            NextTicketNo = nextTicket ?? ""
        //        });

        //        return Json(new { success = true });
        //    }

        //    return Json(new { success = false });
        //}

        [HttpPost]
        public async Task<IActionResult> Call([FromBody] QueueActionRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.PatientIdentification)
                    || string.IsNullOrEmpty(request.Department) || request.VisitDate == default)
                {
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var queue = _dbContext.Queues.FirstOrDefault(q =>
                    q.PatientIdentification == request.PatientIdentification &&
                    q.VisitDate.Date == request.VisitDate.Date &&
                    q.Department == request.Department);

                if (queue == null)
                {
                    return NotFound(new { success = false, message = "Queue entry not found." });
                }

                //Update status to "Serving"
                queue.Status = "Serving";
                _dbContext.SaveChanges();

                string currentTicket = $"{queue.DeptCode}{queue.QueueNumber:D3}";

                var next = _dbContext.Queues
                    .Where(q =>
                        q.Department == request.Department &&
                        q.VisitDate.Date == DateTime.Today &&
                        q.Status == "Standby" &&
                        q.QueueNumber > queue.QueueNumber)
                    .OrderBy(q => q.QueueNumber)
                    .FirstOrDefault();

                string? nextTicket = next != null
                    ? $"{next.DeptCode}{next.QueueNumber:D3}"
                    : null;

                await _hubContext.Clients.All.SendAsync("CallPatient", new
                {
                    PatientName = queue.PatientName,
                    TicketNo = currentTicket,
                    Department = queue.Department,
                    NextTicketNo = nextTicket ?? ""
                });

                await _hubContext.Clients.All.SendAsync("UpdateQueue");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An internal error occurred." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Skip([FromBody] QueueActionRequest request)
        {
            var queue = _dbContext.Queues
                .FirstOrDefault(q => q.PatientIdentification == request.PatientIdentification &&
                                     q.VisitDate.Date == request.VisitDate.Date &&
                                     q.Department == request.Department);

            if (queue != null)
            {
                queue.Status = "Done";
                _dbContext.SaveChanges();

                await _hubContext.Clients.All.SendAsync("UpdateQueue");

                return Json(new { success = true, removeRow = true });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public async Task<IActionResult> Standby([FromBody] QueueActionRequest request)
        {
            var queue = _dbContext.Queues
                .FirstOrDefault(q => q.PatientIdentification == request.PatientIdentification &&
                                     q.VisitDate.Date == request.VisitDate.Date &&
                                     q.Department == request.Department);

            if (queue != null)
            {
                queue.Status = "Standby";
                _dbContext.SaveChanges();

                await _hubContext.Clients.All.SendAsync("UpdateQueue");
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> Next([FromBody] QueueActionRequest request)
        {
            var queue = _dbContext.Queues
                .FirstOrDefault(q => q.PatientIdentification == request.PatientIdentification &&
                                     q.VisitDate.Date == request.VisitDate.Date &&
                                     q.Department == request.Department);

            if (queue != null)
            {
                queue.Status = "Done";
                _dbContext.SaveChanges();

                await _hubContext.Clients.All.SendAsync("UpdateQueue");
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Missed([FromBody] QueueActionRequest request)
        {
            var queue = _dbContext.Queues.FirstOrDefault(q =>
                q.PatientIdentification == request.PatientIdentification &&
                q.VisitDate.Date == request.VisitDate.Date &&
                q.Department == request.Department);

            if (queue != null)
            {
                queue.Status = "Missed";
                _dbContext.SaveChanges();

                await _hubContext.Clients.All.SendAsync("UpdateQueue");

                return Json(new { success = true, removeRow = false }); // keep in list or show in Missed table
            }

            return Json(new { success = false, message = "Patient not found in queue." });
        }

        public IActionResult MissedList()
        {
            var missed = _dbContext.Queues
                .Where(q => q.Status == "Missed" && q.VisitDate.Date == DateTime.Today)
                .OrderBy(q => q.QueueNumber)
                .ToList();

            return PartialView("_MissedQueueRows", missed); // Use a dedicated partial view
        }
    }
}
