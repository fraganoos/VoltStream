namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Commons.Enums;
using VoltStream.WPF.Configurations;

public partial class ApiConnectionViewModel : ViewModelBase
{
    [ObservableProperty] private string url = string.Empty;
    [ObservableProperty] private bool isConnected;
    [ObservableProperty] private bool autoReconnectEnabled = true;
    [ObservableProperty] private bool showIndicator;
    [ObservableProperty] private bool checkUrlEnabled;
    [ObservableProperty] private bool isHttps;
    [ObservableProperty] private string host = "localhost";
    [ObservableProperty] private int port = 5000;
    [ObservableProperty] private string statusText = string.Empty;
    [ObservableProperty] private ConnectionStatus status;

    #region Commands

    [RelayCommand]
    private async Task SaveConnectionSettings()
    {
        try
        {
            Error = string.Empty;
            Success = string.Empty;

            var scheme = IsHttps ? "https" : "http";
            var candidateUrl = $"{scheme}://{Host}:{Port}";

            if (!Uri.TryCreate(candidateUrl, UriKind.Absolute, out var uri))
            {
                Error = "Kiritilgan manzil yaroqsiz";
                return;
            }

            Url = uri.ToString();
            Status = ConnectionStatus.Connecting;

            IsConnected = await App.Services!
                .GetRequiredService<ConnectionTester>()
                .TestAsync();

            Status = IsConnected
                ? ConnectionStatus.Connected
                : ConnectionStatus.Disconnected;

            if (IsConnected)
            {
                Success = "✓ Sozlamalar saqlandi va server bilan aloqa o'rnatildi";
            }
            else
            {
                Error = "✗ Sozlamalar saqlandi, lekin server bilan bog'lanib bo'lmadi";
            }
        }
        catch (Exception ex)
        {
            Error = $"Xatolik: {ex.Message}";
            Status = ConnectionStatus.Disconnected;
        }
    }

    [RelayCommand]
    private void ToggleAutoReconnect() => AutoReconnectEnabled = !AutoReconnectEnabled;

    [RelayCommand]
    private void ToggleCheckUrl() => CheckUrlEnabled = !CheckUrlEnabled;

    [RelayCommand]
    private void ToggleShowIndicator() => ShowIndicator = !ShowIndicator;

    #endregion Commands

    #region PropertyChanged

    partial void OnCheckUrlEnabledChanged(bool value)
    {
        ShowIndicator = value;
    }

    partial void OnAutoReconnectEnabledChanged(bool value)
    {
        UpdateStatus();
    }

    partial void OnIsConnectedChanged(bool value)
    {
        if (value)
            Success = "Aloqa tiklandi";
        else
            Warning = "Aloqa tiklanmadi";

        UpdateStatus();
    }

    partial void OnStatusChanged(ConnectionStatus value)
    {
        StatusText = Status switch
        {
            ConnectionStatus.Connected => "Ulangan",
            ConnectionStatus.Disconnected => "Uzilgan",
            ConnectionStatus.Connecting => "Ulanmoqda...",
            _ => string.Empty
        };

        OnPropertyChanged(nameof(StatusText));
    }

    partial void OnUrlChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            IsHttps = uri.Scheme == "https";
            Host = uri.Host;
            Port = uri.Port;
        }
    }

    #endregion PropertyChanged

    #region Private helper

    private void UpdateStatus()
    {
        Status = IsConnected
            ? ConnectionStatus.Connected
            : AutoReconnectEnabled
                ? ConnectionStatus.Connecting
                : ConnectionStatus.Disconnected;
    }

    #endregion Private helper
}