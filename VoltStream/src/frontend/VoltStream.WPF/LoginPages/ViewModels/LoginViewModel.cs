namespace VoltStream.WPF.LoginPages.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoltStream.WPF.Commons;

public partial class LoginViewModel(ILoginApi loginApi) : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    public event Action? LoginSucceeded;

    [RelayCommand]
    private async Task Login()
    {
        LoginRequest credentials = new() { Username = Username, Password = Password };

        var res = await loginApi.LoginAsync(credentials)
            .Handle(loading => IsLoading = loading);

        if (res.IsSuccess) LoginSucceeded?.Invoke();
        else Error = res.Message ?? "Invalid username or password!";
    }
}