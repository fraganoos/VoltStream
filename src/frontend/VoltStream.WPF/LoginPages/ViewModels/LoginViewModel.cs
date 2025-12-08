namespace VoltStream.WPF.LoginPages.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Configurations;
using VoltStream.WPF.Settings.ViewModels;
using VoltStream.WPF.Settings.Views;

public partial class LoginViewModel(
    ILoginApi loginApi,
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
        try
        {
            // Input validatsiyasi
            if (string.IsNullOrWhiteSpace(Username))
            {
                Error = "Foydalanuvchi nomini kiriting";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Error = "Parolni kiriting";
                return;
            }

            IsLoading = true;
            Error = string.Empty;

            // 1. Server bilan bog'lanishni tekshirish
            var connectionTester = services.GetRequiredService<ConnectionTester>();
            var isConnected = await connectionTester.TestAsync();

            // 2. Agar bog'lanish yo'q bo'lsa, sozlamalar oynasini ochish
            if (!isConnected)
            {
                Error = "⚠ Server bilan bog'lanib bo'lmadi";

                await Task.Delay(1000); // Xabarni ko'rsatish
                IsLoading = false;

                var result = OpenConnectionSettings();

                if (result != true)
                {
                    Error = "Aloqa sozlanmadi. Iltimos, qaytadan urinib ko'ring.";
                    return;
                }

                // Sozlamalar saqlangandan keyin qayta tekshirish
                IsLoading = true;
                var recheckResult = await connectionTester.TestAsync();

                if (!recheckResult)
                {
                    IsLoading = false;
                    Error = "Server bilan bog'lanish hali ham mavjud emas.";
                    return;
                }
            }

            // 3. Bog'lanish muvaffaqiyatli bo'lsa, login qilish
            LoginRequest credentials = new()
            {
                Username = Username,
                Password = Password
            };

            var loginResult = await loginApi.LoginAsync(credentials)
                .Handle(loading => IsLoading = loading);

            if (loginResult.IsSuccess)
            {
                LoginSucceeded?.Invoke();
            }
            else
            {
                Error = loginResult.Message ?? "Noto'g'ri foydalanuvchi nomi yoki parol!";
            }
        }
        catch (HttpRequestException)
        {
            IsLoading = false;
            Error = "Tarmoq xatosi. Server bilan bog'lanib bo'lmadi.";

            // Tarmoq xatosi bo'lsa ham sozlamalar oynasini taklif qilish
            var result = MessageBox.Show(
                "Server bilan bog'lanishda muammo. Sozlamalarni tekshirasizmi?",
                "Bog'lanish xatosi",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                OpenConnectionSettings();
            }
        }
        catch (Exception ex)
        {
            IsLoading = false;
            Error = $"Xatolik yuz berdi: {ex.Message}";
        }
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