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
        public IActionResult DoctorControl(string? search, DateTime? date)
        {
            var today = DateTime.Today;

            // 🟢 Today's queue
            var todayQueue = _dbContext.Queues
                .Where(q => q.VisitDate.Date == today && q.Status == "Done")
                .OrderBy(q => q.QueueNumber)
                .ToList();

            // 📘 Backtrack history
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


        [HttpPost]
        public async Task<IActionResult> SaveDoctorNote(DoctorNote note)
        {
            if (!ModelState.IsValid)
            {
                TempData["DoctorNoteError"] = "Invalid input. Please fill out all fields.";
                return RedirectToAction("DoctorControl");
            }

            // Set Doctor name from logged-in user
            note.DoctorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name;

            try
            {
                // Fetch the exact patient record from Queues by PatientIdentification and latest VisitDate
                var queueRecord = _dbContext.Queues
                    .Where(q => q.PatientIdentification == note.PatientIdentification)
                    .OrderByDescending(q => q.VisitDate)
                    .FirstOrDefault();

                if (queueRecord == null)
                {
                    TempData["DoctorNoteError"] = "Patient record not found.";
                    return RedirectToAction("DoctorControl");
                }

                // Assign PatientName and VisitDate to the note
                note.PatientName = queueRecord.PatientName;
                note.VisitDate = queueRecord.VisitDate;

                // Save note
                _dbContext.DoctorNotes.Add(note);
                await _dbContext.SaveChangesAsync();

                TempData["DoctorNoteSuccess"] = "Diagnosis and prescriptions saved successfully.";
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                TempData["DoctorNoteError"] = "An error occurred while saving the note.";
            }

            return RedirectToAction("DoctorControl");
        }


        //To search patient
        public async Task<IActionResult> Backtrack(string? search, DateTime? date)
        {
            var query = _dbContext.Queues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(q =>
                    q.PatientName.Contains(search) ||
                    q.DeptCode.Contains(search) ||
                    ($"{q.DeptCode}{q.QueueNumber:D3}").Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(q => q.VisitDate.Date == date.Value.Date);
            }

            var model = new PatientHistoryViewModel
            {
                History = await query.OrderByDescending(q => q.VisitDate).ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorNote(string patientId, DateTime visitDate)
        {
            var note = await _dbContext.DoctorNotes
                .Where(d => d.PatientIdentification == patientId && d.VisitDate.Date == visitDate.Date)
                .FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound(new { success = false, message = "No record found." });
            }

            return Json(new
            {
                success = true,
                patientName = note.PatientName,
                visitDate = note.VisitDate.ToString("MMM dd, yyyy"),
                diagnosis = note.Diagnosis,
                prescription = note.Prescription
            });
        }




    }
}

