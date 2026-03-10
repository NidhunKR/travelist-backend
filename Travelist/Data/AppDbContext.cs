using Travelist.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Travelist.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<FlightBooking> FlightBookings { get; set; }



    }
}