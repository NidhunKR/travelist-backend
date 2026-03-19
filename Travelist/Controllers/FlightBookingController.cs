using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travelist.Data;
using Travelist.Models;
using System.Security.Claims;

namespace Travelist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightBookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FlightBookingController(AppDbContext context)
        {
            _context = context;
        }

        // ✈ BOOK FLIGHT

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookFlight(FlightBooking booking)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim missing");
                }

                var userId = int.Parse(userIdClaim.Value);

                booking.UserId = userId;
                booking.Status = "Pending";
                booking.BookingDate = DateTime.UtcNow;

                _context.FlightBookings.Add(booking);
                await _context.SaveChangesAsync();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 📄 GET MY BOOKINGS
        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirst("nameid").Value);

            var bookings = await _context.FlightBookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Flight)
                .ToListAsync();

            return Ok(bookings);
        }

        // ❌ CANCEL BOOKING
        [Authorize]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.FlightBookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return Ok(booking);
        }
    }
}