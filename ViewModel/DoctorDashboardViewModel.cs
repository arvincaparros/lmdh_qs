using LMDH_QS.Models;

namespace LMDH_QS.ViewModel
{
    public class DoctorDashboardViewModel
    {
        public List<Queue> TodayQueue { get; set; }
        public List<Queue> History { get; set; }

        public int PatientIdentification { get; set; } // for unique modal ID
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
    }
}
