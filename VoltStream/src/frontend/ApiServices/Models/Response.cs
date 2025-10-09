namespace ApiServices.Models;

using System.Text.Json.Serialization;

public record Response<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    [JsonIgnore]
    public bool IsSuccess => StatusCode == 200 && Data is not null;
}