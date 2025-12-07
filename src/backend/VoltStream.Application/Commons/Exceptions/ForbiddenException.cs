namespace VoltStream.Application.Commons.Exceptions;

using System;
using System.Net;

[Serializable]
internal class ForbiddenException(string? message) : AppException(message, HttpStatusCode.Forbidden);