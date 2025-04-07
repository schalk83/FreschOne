using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foUserGroups
    {
        public long ID { get; set; }

        [Required]
        public long UserID { get; set; }

        [Required]
        public long GroupID { get; set; }

        // Optional display convenience (not stored in DB)
        public string GroupName { get; set; }
    }
}
