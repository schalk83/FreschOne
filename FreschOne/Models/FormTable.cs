namespace FreschOne.Models
{
    public class FormTable
    {
        public string TableName { get; set; }
        public Dictionary<string, FieldDefinition> Fields { get; set; } = new();
    }
}
