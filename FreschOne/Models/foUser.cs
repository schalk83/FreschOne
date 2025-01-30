namespace FreschOne.Models
{
    public class FoUser
    {
        public long ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
        public bool Active { get; set; }
    }
}
