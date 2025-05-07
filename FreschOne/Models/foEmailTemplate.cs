namespace FreschOne.Models
{
    public class foEmailTemplate
    {
        public long ID { get; set; }
        public string? TemplateType { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
        public DateTime? ActiveDate { get; set; }
        public bool Active { get; set; }
    }
}
