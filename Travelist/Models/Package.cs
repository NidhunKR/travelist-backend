using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelist.Models;

namespace Travelist.Models
{
    public class Package
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public string Details { get; set; }
        public string? ImageUrl { get; set; }


        // Foreign Key
        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination? Destination { get; set; }

        public List<Booking>? Bookings { get; set; }
    }
}