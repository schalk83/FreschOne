namespace FreschOne.Models
{
    public class TableViewModel
    {
        public int UserId { get; set; }
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> TableData { get; set; }
        public string PrimaryKeyColumn { get; set; }
    }

}
