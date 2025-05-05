namespace FreschOne.Models
{
    public class ProcessEventAuditViewModel
    {
        public int DetailID { get; set; }

        public int ProcessEventID { get; set; }
        public decimal StepNo { get; set; }
        public string TableName { get; set; }
        public int RecordID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUserID { get; set; }

        public string FullName {  get; set; }
        // Dictionary containing dynamic field values (deserialized from JSON)
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }

}
