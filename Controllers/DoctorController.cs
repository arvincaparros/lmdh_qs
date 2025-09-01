using LMDH_QS.Models;
using LMDH_QS.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMDH_QS.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _dbContext;

        public DoctorController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public IActionResult DoctorControl(string? search, DateTime? date)
        //{
        //    var today = DateTime.Today;

        //    // 🟢 Today's queue
        //    var todayQueue = _dbContext.Queues
        //        .Where(q => q.VisitDate.Date == today && q.Status == "Consultation Standby")
        //        .OrderBy(q => q.QueueNumber)
        //        .ToList();

        //    // 📘 Backtrack history
        //    var historyQuery = _dbContext.Queues.AsQueryable();

        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        historyQuery = historyQuery.Where(q =>
        //            q.PatientName.Contains(search) ||
        //            q.DeptCode.Contains(search) ||
        //            q.QueueNumber.ToString().Contains(search) ||
        //            q.Department.Contains(search));
        //    }

        //    if (date.HasValue)
        //    {
        //        historyQuery = historyQuery.Where(q => q.VisitDate.Date == date.Value.Date);
        //    }

        //    var history = historyQuery
        //        .OrderByDescending(q => q.VisitDate)
        //        .ThenBy(q => q.QueueNumber)
        //        .Take(100)
        //        .ToList();

        //    var vm = new DoctorDashboardViewModel
        //    {
        //        TodayQueue = todayQueue,
        //        History = history
        //    };

        //    return View(vm);
        //}

        public IActionResult DoctorControl(string? search, DateTime? date)
        {
            var today = DateTime.Today;

            // 🔎 Get logged-in user's department
            var userDept = User.FindFirst("Department")?.Value;

            // 🟢 Today's queue (filtered by department)
            var todayQueue = _dbContext.Queues
                .Where(q => q.VisitDate.Date == today &&
                            q.Status == "Consultation Serving" &&
                            (userDept == null || q.Department == userDept)) // filter by dept
                .OrderBy(q => q.QueueNumber)
                .ToList();

            // 📘 Backtrack history (also filtered if needed)
            var historyQuery = _dbContext.Queues.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                historyQuery = historyQuery.Where(q =>
                    q.PatientName.Contains(search) ||
                    q.DeptCode.Contains(search) ||
                    q.QueueNumber.ToString().Contains(search) ||
                    q.Department.Contains(search));
            }

            if (date.HasValue)
            {
                historyQuery = historyQuery.Where(q => q.VisitDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(userDept))
            {
                historyQuery = historyQuery.Where(q => q.Department == userDept);
            }

            var history = historyQuery
                .OrderByDescending(q => q.VisitDate)
                .ThenBy(q => q.QueueNumber)
                .Take(100)
                .ToList();

            var vm = new DoctorDashboardViewModel
            {
                TodayQueue = todayQueue,
                History = history
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetVitals(string patientId, DateTime visitDate)
        {

            var vital = _dbContext.PatientVitalRecord
                .FirstOrDefault(v => v.PatientId == patientId
                      && v.VisitDate.Date == visitDate.Date);

            if (vital == null)
            {
                return Json(new { success = false, message = "No vital record found." });
            }

            return Json(new
            {
                success = true,
                bp = vital.BP,
                cr = vital.CR,
                pr = vital.PR,
                temp = vital.Temp,
                weight = vital.Weight
            });
        }


        [HttpPost]
        public async Task<IActionResult> SaveDoctorNote(DoctorNote note)
        {
            if (!ModelState.IsValid)
            {
                TempData["DoctorNoteError"] = "Invalid input. Please fill out all required fields.";
                return RedirectToAction("DoctorControl");
            }

            // ✅ Ensure Doctor name always comes from logged-in user
            note.DoctorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name;

            try
            {
                // 🔹 Get patient queue record
                var queueRecord = _dbContext.Queues
                    .Where(q => q.PatientIdentification == note.PatientIdentification && q.VisitDate.Date == note.VisitDate.Date)
                    .OrderByDescending(q => q.VisitDate)
                    .FirstOrDefault();

                if (queueRecord == null)
                {
                    TempData["DoctorNoteError"] = "Patient record not found.";
                    return RedirectToAction("DoctorControl");
                }

                // 🔹 Assign patient data
                note.PatientName = queueRecord.PatientName;
                note.VisitDate = queueRecord.VisitDate;
 

                // ✅ Handle multiple dispositions from checkbox list
                var dispositions = Request.Form["Disposition"];
                if (dispositions.Count > 0)
                {
                    note.Disposition = string.Join(", ", dispositions);
                }

                // 🔹 Save record
                _dbContext.DoctorNotes.Add(note);

                // ✅ Update queue status to Done
                queueRecord.Status = "Done";
                _dbContext.Queues.Update(queueRecord);

                await _dbContext.SaveChangesAsync();

                TempData["DoctorNoteSuccess"] = "Doctor's note saved successfully, and patient status updated to Done.";
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["DoctorNoteError"] = $"An error occurred: {errorMessage}";
            }

            return RedirectToAction("DoctorControl");
        }





        //To search patient
        //public async Task<IActionResult> Backtrack(string? search, DateTime? date)
        //{
        //    var query = _dbContext.Queues.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(q =>
        //            q.PatientName.Contains(search) ||
        //            q.DeptCode.Contains(search) ||
        //            ($"{q.DeptCode}{q.QueueNumber:D3}").Contains(search));
        //    }

        //    if (date.HasValue)
        //    {
        //        query = query.Where(q => q.VisitDate.Date == date.Value.Date);
        //    }

        //    var model = new PatientHistoryViewModel
        //    {
        //        History = await query.OrderByDescending(q => q.VisitDate).ToListAsync()
        //    };

        //    return View(model);
        //}

        // Controller
        [HttpGet]
        public IActionResult BacktrackPartial(string search, DateTime? date)
        {
            var query = _dbContext.Queues.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(q => q.PatientName.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(q => q.VisitDate.Date == date.Value.Date);
            }

            var history = query
                .OrderByDescending(q => q.VisitDate)
                .ToList();

            return PartialView("_BacktrackTable", history);
        }



        [HttpGet]
        public async Task<IActionResult> GetDoctorNote(string patientId, DateTime visitDate)
        {
            try
            {
                // ✅ Compare only the date part in SQL
                var note = await _dbContext.DoctorNotes
                    .FirstOrDefaultAsync(d => d.PatientIdentification == patientId
                        && EF.Functions.DateDiffDay(d.VisitDate, visitDate) == 0);

                if (note == null)
                {
                    return Json(new { success = false, message = "No record found." });
                }

                var vital = await _dbContext.PatientVitalRecord
                    .FirstOrDefaultAsync(v => v.PatientId == patientId
                        && EF.Functions.DateDiffDay(v.VisitDate, visitDate) == 0);

                return Json(new
                {
                    success = true,
                    patientName = note.PatientName,
                    visitDate = note.VisitDate.ToString("MMM dd, yyyy"),
                    bp = vital?.BP,
                    cr = vital?.CR,
                    pr = vital?.PR,
                    temp = vital?.Temp,
                    weight = vital?.Weight,
                    history = note.HistoryOfIllness,
                    diagnosis = note.Diagnosis,
                    remarks = note.Remarks,
                    doctorName = note.DoctorName,
                    dischargeDate = note.DischargeDate?.ToString("MMM dd, yyyy"),
                    dischargeTime = note.DischargeTime?.ToString(@"hh\:mm"),
                    disposition = note.Disposition
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while loading doctor note.",
                    error = ex.GetType().Name,
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }




    }
}

