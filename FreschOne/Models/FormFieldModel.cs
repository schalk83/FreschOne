using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FreschOne.Models
{
    public class FormFieldModel
    {
        public string Column { get; set; }
        public string TableName { get; set; }
        public string ColumnValue { get; set; }
        public string ColumnType { get; set; }
        public int ColumnLength { get; set; }

        public Dictionary<string, List<ForeignKeyInfo>> ForeignKeys { get; set; }
        public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; }
    }
}