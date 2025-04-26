using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using FreschOne.Models;

namespace FreschOne.Models
{
    public class DynamicTableViewModel
    {
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public int RowCount { get; set; }
        public int RenderRowIndex { get; set; }


        // New required properties
        public Dictionary<string, string> ColumnTypes { get; set; } = new();
        public Dictionary<string, int> ColumnLengths { get; set; } = new();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
        public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; } = new();
    }
}