namespace FreschOne.Models
{
    public class foApprovals
    {
        public long ID { get; set; }
        public long ProcessInstanceID { get; set; }
        public long ApprovalEventID { get; set; }
        public long StepID { get; set; }

        public string Decision { get; set; }  // "Approve", "Decline", "Rework"
        public string Comment { get; set; }

        public bool Active { get; set; }
        public int CreatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

}
