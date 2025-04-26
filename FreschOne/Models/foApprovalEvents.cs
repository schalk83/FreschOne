namespace FreschOne.Models
{
    public class foApprovalEvents
    {
        public int ID { get; set; }
        public long ProcessInstanceID { get; set; }
        public long StepID { get; set; }
        public long PreviousEventID { get; set; }
        public long? GroupID { get; set; }
        public long? UserID { get; set; }
        public DateTime? DateAssigned { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}
