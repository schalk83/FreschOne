using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Models
{
    public class TableCreateViewModel
    {
        public string TableName { get; set; }  // Table name
        public List<string> Columns { get; set; }  // List of columns for the table
        public Dictionary<string, string> ColumnTypes { get; set; }  // Column types (varchar, int, etc.)
        public Dictionary<string, int> ColumnLengths { get; set; }  // Column lengths (for varchar fields)
        public List<ForeignKeyInfo> ForeignKeys { get; set; }  // Foreign key columns
        public Dictionary<string, object> Record { get; set; }  // The form data for the new record (not including ID)
        public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; }  // Foreign key options for dropdowns
    }
}
