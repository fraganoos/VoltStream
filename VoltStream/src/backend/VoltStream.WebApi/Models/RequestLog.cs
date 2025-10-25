namespace VoltStream.WebApi.Models;

public record RequestLog(
    string? IpAddress,
    string? Path,
    string? Method,
    string? UserAgent,
    DateTime TimeStamp,
    int? StatusCode,
    bool IsSuccess,
    long? ElapsedMs);
