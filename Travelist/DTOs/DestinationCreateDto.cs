using Microsoft.AspNetCore.Http;

public class DestinationCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }
}