using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FreschOne.Models
{
    public class DynamicTableViewModel
    {
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public int RowCount { get; set; }

        public int StartIndex { get; set; } = 0; // ✅ Rename RenderRowIndex ➔ StartIndex (more clear for row generation)

        public Dictionary<string, string> ColumnTypes { get; set; } = new();
        public Dictionary<string, int> ColumnLengths { get; set; } = new();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
        public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; } = new();
    }
}
