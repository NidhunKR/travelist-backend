using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travelist.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime TravelDate { get; set; }

        public string Status { get; set; } = "Pending";

        // Foreign Keys
        public int UserId { get; set; }
        public int PackageId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("PackageId")]
        public Package? Package { get; set; }
    }
}