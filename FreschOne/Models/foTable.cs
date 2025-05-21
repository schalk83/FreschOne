using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foTable
    {
        public long ID { get; set; }

        [Required]
        [MaxLength(120)]
        public string SchemaName { get; set; }

        [Required]
        [MaxLength(1500)]
        [RegularExpression(@"^[A-Za-z_][A-Za-z0-9_]*$", ErrorMessage = "Only alphanumeric and underscore characters are allowed.")]

        public string TableName { get; set; }

        public string ColumnNames { get; set; }

        public string Script { get; set; }

        public string TableGroup { get; set; } 


        // 🔁 Optional: list of all columns associated with this table
        public List<foTableColumns> Columns { get; set; } = new List<foTableColumns>();
    }
}
