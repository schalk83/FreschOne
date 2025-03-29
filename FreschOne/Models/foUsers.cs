using System.ComponentModel.DataAnnotations;


namespace FreschOne.Models
{
    public class foUser
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Please enter a username")]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter a First Name")]

        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a Last Name")]

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter a Email Name")]

        public string? Email { get; set; }
        public bool Admin { get; set; }
        public bool Active { get; set; }
    }
}
