namespace FreschOne.Models
{
    public class TableViewModel
    {
        public int UserId { get; set; }
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> TableData { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public List<ForeignKeyInfo> ForeignKeys { get; set; }  // List of foreign key columns (for dropdowns)

        public int PageNumber { get; set; }  // Current page number
        public int TotalPages { get; set; }  // Total number of pages
        public string TableDescription { get; set; }
    }

}
