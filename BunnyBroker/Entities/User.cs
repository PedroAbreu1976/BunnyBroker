using System.ComponentModel.DataAnnotations;


namespace BunnyBroker.Entities
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        public string PasswordHash { get; set; } 
    }
}
