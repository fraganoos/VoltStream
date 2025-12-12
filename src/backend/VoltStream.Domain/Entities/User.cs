namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class User : Auditable
{
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;

    public UserRole Role { get; set; }
}
