namespace ApiServices.Models;

using System.Text.Json.Serialization;

public class Response<T>
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonIgnore]
    public bool IsSuccess => StatusCode == 200 && Data is not null;
}