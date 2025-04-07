using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foGroups
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Group description is required.")]
        [Display(Name = "Group Description")]
        public string Description { get; set; }
    }
}
