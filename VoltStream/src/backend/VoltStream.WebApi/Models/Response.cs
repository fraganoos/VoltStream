namespace VoltStream.WebApi.Models;

public class Response
{
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "Ok";
    public object? Data { get; set; }
}
