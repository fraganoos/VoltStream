namespace ApiServices.Models.Requests;

public record CategoryRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}