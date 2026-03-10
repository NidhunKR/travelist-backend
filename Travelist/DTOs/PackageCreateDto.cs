public class PackageCreateDto
{
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public required string Details { get; set; }
    public int DestinationId { get; set; }

    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }   // 👈 ADD THIS
}