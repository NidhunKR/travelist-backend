using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Travelist.Models
{
    public class Flight
    {
        public int Id { get; set; }

        public string Airline { get; set; }

        public string DepartureCity { get; set; }

        public string ArrivalCity { get; set; }

        public decimal Price { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        [JsonIgnore]   // ⭐ IMPORTANT
        public Destination? Destination { get; set; }
    }
}