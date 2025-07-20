using System.ComponentModel.DataAnnotations;

namespace LMDH_QS.Models
{
    public class Queue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? PatientIdentification { get; set; }  // Foreign key reference to PatientsInformation

        public string? PatientName{ get; set; }  // Foreign key reference to PatientsInformation

        [Required]
        public int QueueNumber { get; set; }   // e.g., 001, 002

        [Required]
        public DateTime VisitDate { get; set; } = DateTime.Today;

        public TimeSpan VisitTime { get; set; }

        public string? Department { get; set; }  // Specialist Optional (e.g., OPD, Dental, etc.)
        public string? DeptCode { get; set; }

        public string Status { get; set; } = "Standby";  // Standby, Serving, Done, Skipped

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
