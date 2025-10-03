namespace VoltStream.WPF.LoginPages.Models;
using VoltStream.WPF.Commons;

public class User : ViewModelBase
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
