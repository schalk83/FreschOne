using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foUserReports
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        public long UserID { get; set; }

        [Required(ErrorMessage = "Please select a report")]
        public long ReportID { get; set; }

        public bool Active { get; set; }
    }
}