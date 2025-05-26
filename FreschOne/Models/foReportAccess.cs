namespace FreschOne.Models
{
    public class foReportAccess
    {

        public long ID { get; set; }
        public long ReportID { get; set; }       // Nullable: allows either user-level or group-level access

        public long? UserID { get; set; }       // Nullable: allows either user-level or group-level access

        public long? GroupID { get; set; }      // Nullable: same reason

        public bool Active { get; set; } = true;
    }
}
