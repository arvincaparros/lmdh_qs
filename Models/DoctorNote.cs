using System.ComponentModel.DataAnnotations;

namespace LMDH_QS.Models
{
    public class DoctorNote
    {
        public int Id { get; set; }

        public string PatientIdentification { get; set; } // FK to Queue

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public string? DoctorName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string PatientName { get; set; }

        public DateTime VisitDate { get; set; }
    }

    //public class DoctorNote
    //{
    //    public int Id { get; set; }

    //    [Required]
    //    public string Diagnosis { get; set; }

    //    [Required]
    //    public string Prescription { get; set; }

    //    [Required]
    //    public string PatientIdentification { get; set; }

    //    [Required]
    //    public DateTime VisitDate { get; set; }

    //    public string PatientName { get; set; }
    //    public string DoctorName { get; set; }
    //}
}
