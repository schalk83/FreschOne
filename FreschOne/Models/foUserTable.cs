using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foUserTable
    {
        public long ID { get; set; }

        [Required]
        public long UserID { get; set; }

        [Required]
        public string TableName { get; set; }

        [Required]
        [Display(Name = "Access")]
        public string ReadWriteAccess { get; set; } // R or RW

        public bool Active { get; set; }
    }
}
