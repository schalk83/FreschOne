namespace FreschOne.Models
{
    public class foApprovalAttachments
    {
        public long ID { get; set; }
        public long ApprovalID { get; set; }  // FK to foApprovals.ID

        public string FileName { get; set; }         // Stored path
        public string AttachmentDescription { get; set; }

        public bool Active { get; set; }
        public int CreatedUserID { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
