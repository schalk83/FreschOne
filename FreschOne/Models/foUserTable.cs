namespace FreschOne.Models
{

    public class foUserTable
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string TableName { get; set; }
        public string ReadWriteAccess { get; set; }
        public bool Active { get; set; }
    }
}