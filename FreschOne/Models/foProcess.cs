
using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foProcess
    {
        public long ID { get; set; }

        [Required]
        public string ProcessName { get; set; }

        public string ProcessDescription { get; set; }

        public bool Active { get; set; }
    }
}
