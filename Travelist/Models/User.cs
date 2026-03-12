using System.ComponentModel.DataAnnotations;
using Travelist.Models;

namespace Travelist.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }  // Admin or User

        // Navigation Property
        public List<Booking> Bookings { get; set; }
    }
}