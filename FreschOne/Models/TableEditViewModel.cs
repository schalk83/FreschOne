using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Models
{
    public class TableEditViewModel
    {
        public string TableName { get; set; }  // The name of the table being edited
        public List<string> Columns { get; set; }  // List of column names for the table
        public Dictionary<string, object> Record { get; set; }  // Current record data, key: column name, value: column value
        public List<ForeignKeyInfo> ForeignKeys { get; set; }  // List of foreign key columns (for dropdowns)
        public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; }  // Options for foreign key dropdowns
    }
}
