
using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foUserProcess
    {
        public long ID { get; set; }

        [Required]
        public long UserID { get; set; }

        [Required]
        public long ProcessID { get; set; }

        public string? ProcessName { get; set; }

    }

}
