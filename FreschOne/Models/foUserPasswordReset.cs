namespace FreschOne.Models
{
    public class FoUserPasswordReset
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public bool ResetWithNextLogin { get; set; }
    }
}
