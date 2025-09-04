namespace VoltStream.Application.Commons.Exceptions;

using System;
using System.Net;

[Serializable]
public class ConflictException(string? message) : AppException(message, HttpStatusCode.Conflict);