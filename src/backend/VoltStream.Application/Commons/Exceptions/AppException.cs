namespace VoltStream.Application.Commons.Exceptions;

using System.Net;

public class AppException(string? message,
    HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}