using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travelist.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        public string HotelName { get; set; }

        public decimal PricePerNight { get; set; }

        public double Rating { get; set; }

        // Foreign Key
        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; }
    }
}