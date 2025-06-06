﻿using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foReportTable
    {
        public long ID { get; set; }

        [Required]
        public long ReportsID { get; set; }

        [Required(ErrorMessage = "Table Name is required.")]
        public string? TableName { get; set; }

        [Required(ErrorMessage = "Column Query is required.")]
        public string? ColumnQuery { get; set; }

        [Required(ErrorMessage = "Form Type is required.")]
        public string? FormType { get; set; }

        [Required(ErrorMessage = "Column Count is required.")]
        public int? ColumnCount { get; set; }

        [Required]
        public bool Parent { get; set; }

        public string? FKColumn { get; set; }

        public string? TableDescription { get; set; }

        public bool? Active { get; set; }
    }
}
