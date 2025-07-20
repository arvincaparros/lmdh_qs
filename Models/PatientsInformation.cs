using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LMDH_QS.Models
{
    public class PatientsInformation
    {
        [Key]
        public int Id { get; set; }

        //[Display(Name = "Patient ID")]
        //[StringLength(20)]
        //public string PatientId { get; set; }  // Format: PAT-YYYYMMDD-XXXX

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Visit")]
        public DateTime VisitDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time of Visit")]
        public TimeSpan VisitTime { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Telephone Number")]
        [RegularExpression(@"^(09\d{9}|\+639\d{9}|\d{7,8})$",
        ErrorMessage = "Enter a valid mobile number (09XXXXXXXXX or +639XXXXXXXXX) or landline number.")]
        public string TelNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime Birthdate { get; set; }

        [Required]
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Marital Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        public int QueueNumber { get; set; }
        public string? PatientIdentity { get; set; }

        public string? Department { get; set; }
        public string? DeptCode { get; set; }

        public bool HasConsented { get; set; }
    }
}
