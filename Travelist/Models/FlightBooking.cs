using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travelist.Models
{
    public class FlightBooking
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        [ForeignKey("FlightId")]
        public Flight? Flight { get; set; }

        public int UserId { get; set; }

        public string PassengerName { get; set; }

        public int Seats { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    }
}