namespace VoltStream.Application.Commons.Exceptions;

using System;
using System.Net;

[Serializable]
public class ConflictException : AppException
{
    public ConflictException(string? message) : base(message, HttpStatusCode.Conflict) { }
}