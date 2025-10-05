namespace VoltStream.WPF.LoginPages.Models;

using ApiServices.DTOs.Users;
using ApiServices.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class LoginViewModel(ILoginApi loginApi) : ObservableObject
{
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private bool isViewVisible = true;
    [ObservableProperty] private bool isBusy = false;

    public event Action? LoginSucceeded;

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var res = await loginApi.LoginAsync(new LoginRequest { Username = Username, Password = Password });
            if (res.IsSuccessStatusCode) LoginSucceeded?.Invoke();
            else ErrorMessage = res.Content?.Message ?? "Invalid username or password!";
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
