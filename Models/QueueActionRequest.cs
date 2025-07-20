namespace LMDH_QS.Models
{
    public class QueueActionRequest
    {
        public string PatientIdentification { get; set; }
        public DateTime VisitDate { get; set; }
        public string Department { get; set; }
    }
}
