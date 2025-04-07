using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foTableColumnsToIgnore
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Column Name is required.")]
        public string? ColumnName { get; set; }
    }
}
