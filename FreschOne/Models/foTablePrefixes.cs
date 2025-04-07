using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foTablePrefixes
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Prefix is required.")]
        public string? Prefix { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Active status is required.")]
        public bool Active { get; set; }
    }
}
