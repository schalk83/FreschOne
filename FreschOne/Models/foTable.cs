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
        public string TableName { get; set; }

        // 🔁 Optional: list of all columns associated with this table
        public List<foTableColumns> Columns { get; set; } = new List<foTableColumns>();
    }
}
