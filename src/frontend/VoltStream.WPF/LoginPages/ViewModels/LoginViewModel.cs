namespace VoltStream.WPF.LoginPages.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Configurations;
using VoltStream.WPF.Settings.ViewModels;
using VoltStream.WPF.Settings.Views;

public partial class LoginViewModel(
    IServiceProvider services) : ViewModelBase
{
    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public event Action? LoginSucceeded;

    [RelayCommand]
    public async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            Warning = "Foydalanuvchi nomini kiriting";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            Warning = "Parolni kiriting";
            return;
        }

        var connectionTester = services.GetRequiredService<ConnectionTester>();
        var isConnected = await connectionTester.TestAsync(isLoading => IsLoading = isLoading);

        if (!isConnected)
        {
            Error = "⚠ Server bilan bog'lanib bo'lmadi";
            await Task.Delay(1000);
            var result = OpenConnectionSettings();

            if (result != true)
            {
                Error = "Aloqa sozlanmadi. Iltimos, qaytadan urinib ko'ring.";
                return;
            }

            var recheckResult = await connectionTester.TestAsync(isLoading => IsLoading = isLoading);

            if (!recheckResult)
            {
                Error = "Server bilan bog'lanish hali ham mavjud emas.";
                return;
            }
        }

        LoginRequest credentials = new()
        {
            Username = Username,
            Password = Password
        };

        var loginApi = services.GetRequiredService<ILoginApi>();
        var loginResult = await loginApi.LoginAsync(credentials)
            .Handle(loading => IsLoading = loading);

        if (loginResult.IsSuccess && loginResult.Data != null)
        {
            var sessionService = services.GetRequiredService<ISessionService>();
            sessionService.CurrentUser = loginResult.Data;
            LoginSucceeded?.Invoke();
        }
        else Error = loginResult.Message ?? "Noto'g'ri foydalanuvchi nomi yoki parol!";
    }

    private bool? OpenConnectionSettings()
    {
        try
        {
            var viewModel = services.GetRequiredService<ConnectionSettingsViewModel>();
            var window = new ConnectionSettingsWindow(viewModel)
            {
                Owner = Application.Current.MainWindow
            };

            return window.ShowDialog();
        }
        catch (Exception ex)
        {
            Error = $"Sozlamalar oynasini ochishda xatolik: {ex.Message}";
            return false;
        }
    }
}