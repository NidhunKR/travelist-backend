using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travelist.Data;
using Travelist.Models;

namespace Travelist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DestinationController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Everyone can see destinations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var destinations = await _context.Destinations.ToListAsync();
            return Ok(destinations);
        }

        // ✅ Get single destination
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
                return NotFound();

            return Ok(destination);
        }

        // ✅ Admin create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DestinationCreateDto dto)
        {
            string? imageUrl = null;

            if (dto.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imageUrl = $"/images/{fileName}";
            }

            var destination = new Destination
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = imageUrl
            };

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return Ok(destination);
        }
        // ✅ Admin update
        // ✅ Admin update
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DestinationCreateDto dto)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
                return NotFound();

            destination.Name = dto.Name;
            destination.Description = dto.Description;

            if (dto.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                destination.ImageUrl = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(destination);
        }

        // ✅ Admin delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
                return NotFound();

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return Ok("Destination deleted");
        }
    }
}