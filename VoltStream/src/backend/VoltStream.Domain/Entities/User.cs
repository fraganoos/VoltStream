namespace VoltStream.Domain.Entities;
public class User : Auditable
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;

}
