using LMDH_QS.Hubs;
using LMDH_QS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LMDH_QS.Controllers
{
    public class PatientRegistrationController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IHubContext<QueueHub> hubContext;


        public PatientRegistrationController(AppDbContext dbContext, IHubContext<QueueHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public IActionResult PatientRegistrationForm()
        {
            return View();
        }

        private int GenerateQueueNumber(DateTime visitDate, string department)
        {
            return dbContext.Queues
                .Count(q => q.VisitDate.Date == visitDate.Date && q.Department.ToLower() == department.ToLower()) + 1;
        }

        private string GeneratePatientIdWithRandom(DateTime visitDate, int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            string randomPart = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            string datePart = visitDate.ToString("yyyyMMdd");

            return $"PAT-{datePart}-{randomPart}";
        }

        [HttpPost]
        public async Task<IActionResult> PatientRegistrationForm(PatientsInformation model)
        {
            model.VisitDate = DateTime.Today;
            model.QueueNumber = GenerateQueueNumber(model.VisitDate, model.Department);

            bool isExisting = false;
            bool isAlreadyInQueue = false;

            // ✅ Validate consent
            if (!model.HasConsented)
            {
                return Json(new
                {
                    success = false,
                    message = "You must give your consent to proceed with registration. (Kailangan mong pumayag upang magpatuloy sa pagpaparehistro.)"
                });
            }

            // Check if patient already exists
            var existingPatient = dbContext.PatientsInformation.FirstOrDefault(p =>
                p.FirstName.ToLower() == model.FirstName.ToLower().Trim() &&
                p.MiddleName.ToLower() == model.MiddleName.ToLower().Trim() &&
                p.LastName.ToLower() == model.LastName.ToLower().Trim() &&
                p.Birthdate.Date == model.Birthdate.Date);

            if (existingPatient != null)
            {
                model.PatientIdentity = existingPatient.PatientIdentity;
                isExisting = true;
            }
            else
            {
                model.PatientIdentity = GeneratePatientIdWithRandom(model.VisitDate);
                dbContext.PatientsInformation.Add(model); // New patient (includes HasConsented)
                dbContext.SaveChanges(); // Save to assign identity
            }

            // Map Department to DeptCode
            string deptCode = model.Department?.ToLower() switch
            {
                "medical" => "M",
                "ob-gyne" => "OB",
                "pedia" => "P",
                "surgeon" => "S",
                "ortho" => "OR",
                _ => ""
            };

            model.DeptCode = deptCode;

            // Check if already in queue for same patient, date, and department
            isAlreadyInQueue = dbContext.Queues.Any(q =>
                q.PatientIdentification == model.PatientIdentity &&
                q.VisitDate.Date == model.VisitDate.Date &&
                q.Department.ToLower() == model.Department.ToLower());

            if (!isAlreadyInQueue)
            {
                // Add to Queue
                var queueEntry = new Models.Queue
                {
                    PatientIdentification = model.PatientIdentity,
                    PatientName = $"{model.FirstName} {model.LastName}".Trim(),
                    QueueNumber = model.QueueNumber,
                    VisitDate = model.VisitDate,
                    VisitTime = model.VisitTime,
                    Department = model.Department,
                    DeptCode = model.DeptCode,
                    Status = "Standby",
                    CreatedAt = DateTime.Now
                };

                dbContext.Queues.Add(queueEntry);
                dbContext.SaveChanges();

                // ✅ Notify SignalR clients
                await hubContext.Clients.All.SendAsync("UpdateQueue");
            }

            var message = isAlreadyInQueue
                ? $"You are already in the queue for {model.Department}. Please wait to be called for Pre-assessment."
                : isExisting
                    ? $"You are already registered. Your queue number is {model.QueueNumber:D2}. Please wait to be called."
                    : $"Patient registered successfully! Your queue number is {model.QueueNumber:D2}. Please wait to be called.";

            return Json(new
            {
                success = true,
                queueNumber = model.QueueNumber,
                patientId = model.PatientIdentity,
                alreadyInQueue = isAlreadyInQueue,
                message = message
            });
        }


        //[HttpPost]
        //public async Task<IActionResult> PatientRegistrationForm(PatientsInformation model)
        //{
        //    model.VisitDate = DateTime.Today;
        //    model.QueueNumber = GenerateQueueNumber(model.VisitDate, model.Department);

        //    bool isExisting = false;
        //    bool isAlreadyInQueue = false;

        //    // Check if patient already exists
        //    var existingPatient = dbContext.PatientsInformation.FirstOrDefault(p =>
        //        p.FirstName.ToLower() == model.FirstName.ToLower().Trim() &&
        //        p.MiddleName.ToLower() == model.MiddleName.ToLower().Trim() &&
        //        p.LastName.ToLower() == model.LastName.ToLower().Trim() &&
        //        p.Birthdate.Date == model.Birthdate.Date);

        //    if (existingPatient != null)
        //    {
        //        model.PatientIdentity = existingPatient.PatientIdentity;
        //        isExisting = true;
        //    }
        //    else
        //    {
        //        model.PatientIdentity = GeneratePatientIdWithRandom(model.VisitDate);
        //        dbContext.PatientsInformation.Add(model); // New patient
        //        dbContext.SaveChanges(); // Save patient to assign identity
        //    }

        //    // Map Department to DeptCode
        //    string deptCode = model.Department?.ToLower() switch
        //    {
        //        "medical" => "M",
        //        "ob-gyne" => "OB",
        //        "pedia" => "P",
        //        "surgeon" => "S",
        //        "ortho" => "OR",
        //        _ => ""
        //    };
        //    model.DeptCode = deptCode;

        //    // Check if already in queue for same patient, date, and department
        //    isAlreadyInQueue = dbContext.Queues.Any(q =>
        //        q.PatientIdentification == model.PatientIdentity &&
        //        q.VisitDate.Date == model.VisitDate.Date &&
        //        q.Department.ToLower() == model.Department.ToLower());

        //    if (!isAlreadyInQueue)
        //    {
        //        // Add to Queue
        //        var queueEntry = new Models.Queue
        //        {
        //            PatientIdentification = model.PatientIdentity,
        //            PatientName = $"{model.FirstName} {model.LastName}".Trim(),
        //            QueueNumber = model.QueueNumber,
        //            VisitDate = model.VisitDate,
        //            VisitTime = model.VisitTime,
        //            Department = model.Department,
        //            DeptCode = model.DeptCode,
        //            Status = "Standby",
        //            CreatedAt = DateTime.Now
        //        };

        //        dbContext.Queues.Add(queueEntry);
        //        dbContext.SaveChanges();

        //        // ✅ Notify SignalR clients
        //        await hubContext.Clients.All.SendAsync("UpdateQueue");
        //    }

        //    var message = isAlreadyInQueue
        //        ? $"You are already in the queue for {model.Department}. Please wait to be called."
        //        : isExisting
        //            ? $"You are already registered. Your queue number is {model.QueueNumber:D2}. Please wait to be called."
        //            : $"Patient registered successfully! Your queue number is {model.QueueNumber:D2}. Please wait to be called.";

        //    return Json(new
        //    {
        //        success = true,
        //        queueNumber = model.QueueNumber,
        //        patientId = model.PatientIdentity,
        //        alreadyInQueue = isAlreadyInQueue,
        //        message = message
        //    });
        //}



    }
}
