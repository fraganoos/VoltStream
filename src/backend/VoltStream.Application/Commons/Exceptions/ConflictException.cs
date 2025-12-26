namespace VoltStream.Application.Commons.Exceptions;

using System.Net;

[Serializable]
public class ConflictException(string? message) : AppException(message, HttpStatusCode.Conflict);