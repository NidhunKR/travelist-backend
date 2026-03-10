using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travelist.Data;
using Travelist.Models;
using Microsoft.AspNetCore.Hosting;

namespace Travelist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PackageController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ✅ Get all packages (Public)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _context.Packages
                .Include(p => p.Destination)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Price,
                    p.DurationInDays,
                    p.Details,
                    p.ImageUrl,
                    DestinationId = p.DestinationId,
                    DestinationName = p.Destination.Name,
                    DestinationImageUrl = p.Destination.ImageUrl
                })
                .ToListAsync();

            return Ok(packages);
        }

        // ✅ Get package by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var package = await _context.Packages
                .Include(p => p.Destination)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Price,
                    p.DurationInDays,
                    p.Details,
                    p.ImageUrl,
                    p.DestinationId,
                    DestinationName = p.Destination.Name,
                    DestinationImageUrl = p.Destination.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (package == null)
                return NotFound();

            return Ok(package);
        }

        // ✅ Get by destination
        [HttpGet("destination/{destinationId}")]
        public async Task<IActionResult> GetByDestination(int destinationId)
        {
            var packages = await _context.Packages
                .Where(p => p.DestinationId == destinationId)
                .ToListAsync();

            return Ok(packages);
        }

        // ✅ CREATE PACKAGE (Admin + Image Upload Support)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromForm] PackageCreateDto dto)
        {
            var destinationExists = await _context.Destinations
                .AnyAsync(d => d.Id == dto.DestinationId);

            if (!destinationExists)
                return BadRequest("Destination does not exist");

            string? imagePath = dto.ImageUrl;

            // ✅ If image file uploaded
            if (dto.ImageFile != null)
            {
                var rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadsFolder = Path.Combine(rootPath, "images");

                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imagePath = $"/images/{fileName}";
            }

            var package = new Package
            {
                Title = dto.Title,
                Price = dto.Price,
                DurationInDays = dto.DurationInDays,
                Details = dto.Details,
                DestinationId = dto.DestinationId,
                ImageUrl = imagePath
            };

            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            return Ok(package);
        }

        // ✅ Update package
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromForm] PackageUpdateDto dto)
        {
            var existingPackage = await _context.Packages.FindAsync(id);

            if (existingPackage == null)
                return NotFound();

            existingPackage.Title = dto.Title;
            existingPackage.Price = dto.Price;
            existingPackage.DurationInDays = dto.DurationInDays;
            existingPackage.Details = dto.Details;
            existingPackage.DestinationId = dto.DestinationId;

            // 🔥 If new image uploaded
            if (dto.ImageFile != null)
            {
                var rootPath = _environment.WebRootPath ??
                               Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadsFolder = Path.Combine(rootPath, "images");

                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                existingPackage.ImageUrl = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(existingPackage);
        }

        // ✅ Delete package
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
                return NotFound();

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();

            return Ok("Package deleted");
        }
    }
}