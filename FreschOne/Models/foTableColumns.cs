using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foTableColumns
    {
        public long ID { get; set; }

        public long TableID { get; set; }

        [Required]
        [MaxLength(200)]
        public string ColumnName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ColumnDataType { get; set; }

        [MaxLength(50)]
        public string ColumnLength_Precision { get; set; }

        public bool IsNullable { get; set; }

        [MaxLength(200)]
        public string ForeignKeyTable { get; set; }

        public bool Attachment { get; set; } = true;

        public bool Geo { get; set; } = true;

        public bool Active { get; set; } = true;


    }
}
