using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travelist.Data;
using Travelist.Models;

namespace Travelist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FlightController(AppDbContext context)
        {
            _context = context;
        }

        // GET all flights
        [HttpGet]
        public async Task<IActionResult> GetFlights(int page = 1, int pageSize = 5)
        {
            var flights = await _context.Flights
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(flights);
        }

        // GET flight by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        // SEARCH flights
        [HttpGet("search")]
        public async Task<IActionResult> SearchFlights(string from, string to, decimal? maxPrice)
        {
            var query = _context.Flights
                .Where(f => f.DepartureCity.ToLower() == from.ToLower() &&
                            f.ArrivalCity.ToLower() == to.ToLower());

            if (maxPrice.HasValue)
            {
                query = query.Where(f => f.Price <= maxPrice.Value);
            }

            var flights = await query.ToListAsync();

            return Ok(flights);
        }

        // ADD flight
        [HttpPost]
        public async Task<IActionResult> AddFlight(Flight flight)
        {
            flight.DepartureTime = DateTime.SpecifyKind(flight.DepartureTime, DateTimeKind.Utc);
            flight.ArrivalTime = DateTime.SpecifyKind(flight.ArrivalTime, DateTimeKind.Utc);

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return Ok(flight);
        }

        // UPDATE flight
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, Flight flight)
        {
            if (id != flight.Id)
                return BadRequest();

            _context.Entry(flight).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(flight);
        }

        // DELETE flight
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
                return NotFound();

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return Ok("Flight deleted");
        }

    }
}