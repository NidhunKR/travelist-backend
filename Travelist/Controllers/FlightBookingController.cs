using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travelist.Data;
using Travelist.Models;

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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookFlight(FlightBooking booking)
        {
            _context.FlightBookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }
        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> MyBookings(int userId)
        {
            var bookings = await _context.FlightBookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Flight)
                .ToListAsync();

            return Ok(bookings);
        }
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