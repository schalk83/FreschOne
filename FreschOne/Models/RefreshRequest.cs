// Models/RefreshRequest.cs
namespace FreschOne.Models
{
    public class RefreshRequest
    {
        public string TableName { get; set; }
        public Dictionary<string, object> Row { get; set; }
    }
}
