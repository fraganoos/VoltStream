namespace VoltStream.WPF.LoginPages.Models;

using ApiServices.DTOs.Users;
using ApiServices.Interfaces;
using global::VoltStream.WPF.Commons;
using System.Windows.Input;

public class LoginViewModel : ViewModelBase
{
    private string _username = "";
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(nameof(Username)); }
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(nameof(Password)); }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
    }

    private bool _isViewVisible = true;
    public bool IsViewVisible
    {
        get => _isViewVisible;
        set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    public ICommand LoginCommand { get; }
    private readonly ILoginApi _loginApi;
    // ✅ Login muvaffaqiyatli bo‘lsa signal beradigan event
    public event Action? LoginSucceeded;

    public LoginViewModel(ILoginApi loginApi)
    {
        _loginApi = loginApi;
        LoginCommand = new ViewModelCommand(async (obj) => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;

            var response = await _loginApi.LoginAsync(new LoginRequest
            {
                Username = this.Username,
                Password = this.Password
            });

            if (response.IsSuccessStatusCode)
            {
                ErrorMessage = "";
                // ✅ Event chaqiriladi
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = response.Content?.Message ?? "Invalid username or password!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

