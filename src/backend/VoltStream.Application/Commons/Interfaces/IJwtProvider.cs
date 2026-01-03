namespace VoltStream.Application.Commons.Interfaces;

using VoltStream.Domain.Entities;

public interface IJwtProvider
{
    string Generate(User user);
}
