namespace FreschOne.Models
{
    public class foUserReports
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long ReportID { get; set; }  // Fixed type from string to long
        public bool Active { get; set; }
    }
}
