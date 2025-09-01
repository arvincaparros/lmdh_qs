using System.ComponentModel.DataAnnotations;

namespace LMDH_QS.Models
{
    public class PatientVitalRecord
    {
        [Key]
        public int Id { get; set; } // Primary Key

        public string? PatientId { get; set; } // Foreign key to Patient table

        // Vital Signs
        public string BP { get; set; }      // e.g. "120/80"
        public int? CR { get; set; }        // Cardiac Rate (nullable)
        public int? PR { get; set; }        // Pulse Rate (nullable)
        public decimal? Temp { get; set; }  // Temperature in °C (nullable)
        public decimal? Weight { get; set; } // Weight in kg (nullable)

        // Metadata
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        public string? RecordedBy { get; set; } // Staff name / userId

        public DateTime VisitDate { get; set; }
    }


}
