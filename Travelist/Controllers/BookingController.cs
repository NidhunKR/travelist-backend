using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Travelist.Data;
using Travelist.Models;

namespace Travelist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require login
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ User books a package
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(int packageId, DateTime travelDate)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var packageExists = await _context.Packages
                .AnyAsync(p => p.Id == packageId);

            if (!packageExists)
                return BadRequest("Package not found");

            var booking = new Booking
            {
                UserId = userId,
                PackageId = packageId,
                TravelDate = DateTime.SpecifyKind(travelDate, DateTimeKind.Utc),
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }

        // ✅ User sees only their bookings
        [HttpGet("my-bookings")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bookings = await _context.Bookings
                .Include(b => b.Package)
                .ThenInclude(p => p.Destination)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.TravelDate,
                    b.Status,
                    Package = new
                    {
                        b.Package.Id,
                        b.Package.Title,
                        b.Package.Price,
                        Destination = b.Package.Destination.Name
                    }
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // ✅ Admin sees all bookings
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Package)
                .ThenInclude(p => p.Destination)
                .Select(b => new
                {
                    b.Id,
                    b.TravelDate,
                    b.Status,
                    User = b.User.Email,
                    Package = b.Package.Title,
                    Destination = b.Package.Destination.Name
                })
                .ToListAsync();

            return Ok(bookings);
        }
        // ✅ Admin updates booking status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] string status)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            // Optional: Validate allowed statuses
            var allowedStatuses = new[] { "Pending", "Confirmed", "Cancelled" };

            if (!allowedStatuses.Contains(status))
                return BadRequest("Invalid status");

            booking.Status = status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                booking.Id,
                booking.Status
            });
        }
    }
}