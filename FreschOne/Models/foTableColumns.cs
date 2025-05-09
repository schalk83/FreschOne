using System;
using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foTableColumns
    {
        public long ID { get; set; }

        [Required]
        public long TableID { get; set; }

        [Required]
        [Display(Name = "Column Name")]
        public string ColumnName { get; set; }

        [Display(Name = "Actual Name")]
        public string? ColumnActualName { get; set; }

        [Display(Name = "Order")]
        public long ColumnOrder { get; set; }

        [Display(Name = "Max Length")]
        public long ColumnMaxLength { get; set; }

        [Display(Name = "Precision")]
        public long ColumnPrecision { get; set; }

        [Display(Name = "IsPrimaryKey")]
        public bool IsPrimaryKey { get; set; }

        [Display(Name = "Nullable")]
        public bool ColumnIsNullable { get; set; }

        [Required]
        [Display(Name = "Data Type")]
        public string ColumnDataType { get; set; }
                
        [Display(Name = "Foreign Key")]
        public bool IsForeignKey { get; set; }

        [Display(Name = "FK Table")]
        public string? ForeignKeyTableName { get; set; }

        public bool IsSystemColumn { get; set; } = false;

        [Display(Name = "Active")]
        public bool Active { get; set; } = true;

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Modified")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        // Not stored in DB — for internal view logic
        public bool Deleted { get; set; } = false;
    }
}
