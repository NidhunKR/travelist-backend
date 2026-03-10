using System.ComponentModel.DataAnnotations;
using Travelist.Models;

namespace Travelist.Models
{
    public class Destination
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        // Navigation Properties
        public List<Package> Packages { get; set; } = new();
        public List<Hotel> Hotels { get; set; } = new();
        public List<Flight> Flights { get; set; } = new();
    }
}