using Microsoft.AspNetCore.Http;

public class PackageUpdateDto
{
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public string Details { get; set; }
    public int DestinationId { get; set; }

    public IFormFile? ImageFile { get; set; }  // 🔥 important
}