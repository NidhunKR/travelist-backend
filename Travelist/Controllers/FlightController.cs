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
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(from))
            {
                query = query.Where(f =>
                    f.DepartureCity.ToLower().Contains(from.ToLower()));
            }

            if (!string.IsNullOrEmpty(to))
            {
                query = query.Where(f =>
                    f.ArrivalCity.ToLower().Contains(to.ToLower()));
            }

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
        // SEED FLIGHTS DATA
        [HttpPost("seed-flights")]
        public async Task<IActionResult> SeedFlights()
        {
            var airlines = new[]
            {
        "Emirates","Qatar Airways","Singapore Airlines",
        "Air India","Etihad Airways","Thai Airways",
        "AirAsia","Garuda Indonesia"
    };

            var cities = new[]
            {
        new {Name="Dubai",Id=3},
        new {Name="Phuket",Id=4},
        new {Name="Singapore",Id=5},
        new {Name="Kerala",Id=6},
        new {Name="Bali",Id=7}
    };

            var flights = new List<Flight>();
            var rand = new Random();

            foreach (var from in cities)
            {
                foreach (var to in cities)
                {
                    if (from.Name == to.Name) continue;

                    foreach (var airline in airlines)
                    {
                        flights.Add(new Flight
                        {
                            Airline = airline,
                            DepartureCity = from.Name,
                            ArrivalCity = to.Name,
                            Price = rand.Next(200, 600),
                            DepartureTime = DateTime.UtcNow.AddDays(rand.Next(1, 30)),
                            ArrivalTime = DateTime.UtcNow.AddDays(rand.Next(1, 30)).AddHours(5),
                            DestinationId = to.Id
                        });
                    }
                }
            }

            _context.Flights.AddRange(flights);
            await _context.SaveChangesAsync();

            return Ok($"{flights.Count} flights added");
        }

    }
}