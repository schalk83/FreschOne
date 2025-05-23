using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foProcessDetail
    {
        public long ID { get; set; }

        public long StepID { get; set; }

        [Required]
        public string TableName { get; set; }

        [Required]
        public string ColumnQuery { get; set; }

        [Required]
        public string FormType { get; set; }

        public string? ListTable { get; set; }

        [Required]
        public int? ColumnCount { get; set; }

        [Required]
        public bool Parent { get; set; }

        public string? FKColumn { get; set; }

        [Required]
        public string TableDescription { get; set; }

        public string ? ColumnCalcs { get; set; }
        public bool Active { get; set; }

    }
}