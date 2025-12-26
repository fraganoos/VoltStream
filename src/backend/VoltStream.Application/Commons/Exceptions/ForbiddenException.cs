namespace VoltStream.Application.Commons.Exceptions;

using System.Net;

[Serializable]
internal class ForbiddenException(string? message) : AppException(message, HttpStatusCode.Forbidden);