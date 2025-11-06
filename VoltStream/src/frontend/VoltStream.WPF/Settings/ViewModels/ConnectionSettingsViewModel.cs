namespace VoltStream.WPF.Settings.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Configurations;

public partial class ConnectionSettingsViewModel : ViewModelBase
{
    private readonly IServiceProvider services;
    private readonly ApiConnectionViewModel apiConnection;
    private Action? closeAction;

    public ConnectionSettingsViewModel(IServiceProvider services)
    {
        this.services = services;
        apiConnection = services.GetRequiredService<ApiConnectionViewModel>();

        // ApiConnectionViewModel propertylarini expose qilish
        SyncFromApiConnection();

        // Property change eventlarini subscribe qilish
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(IsHttps) or nameof(Host) or nameof(Port))
            {
                UpdatePreviewUrl();
            }
        };
    }

    public void SetCloseAction(Action closeAction)
    {
        this.closeAction = closeAction;
    }

    #region Properties - ApiConnectionViewModel bilan sync

    public bool IsHttps
    {
        get => apiConnection.IsHttps;
        set => apiConnection.IsHttps = value;
    }

    public string Host
    {
        get => apiConnection.Host;
        set => apiConnection.Host = value;
    }

    public int Port
    {
        get => apiConnection.Port;
        set => apiConnection.Port = value;
    }

    public bool AutoReconnectEnabled
    {
        get => apiConnection.AutoReconnectEnabled;
        set => apiConnection.AutoReconnectEnabled = value;
    }

    public bool CheckUrlEnabled
    {
        get => apiConnection.CheckUrlEnabled;
        set => apiConnection.CheckUrlEnabled = value;
    }

    public bool ShowIndicator
    {
        get => apiConnection.ShowIndicator;
        set => apiConnection.ShowIndicator = value;
    }

    public string Url => apiConnection.Url;

    public string StatusText => apiConnection.StatusText;


    #endregion

    private void SyncFromApiConnection()
    {
        OnPropertyChanged(nameof(IsHttps));
        OnPropertyChanged(nameof(Host));
        OnPropertyChanged(nameof(Port));
        OnPropertyChanged(nameof(AutoReconnectEnabled));
        OnPropertyChanged(nameof(CheckUrlEnabled));
        OnPropertyChanged(nameof(ShowIndicator));
        OnPropertyChanged(nameof(Url));
        OnPropertyChanged(nameof(StatusText));
    }

    private void UpdatePreviewUrl()
    {
        var scheme = IsHttps ? "https" : "http";
        var previewUrl = $"{scheme}://{Host}:{Port}";
        OnPropertyChanged(nameof(Url));
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        try
        {
            Error = string.Empty;
            Success = string.Empty;
            Warning = string.Empty;
            IsLoading = true;

            // URL validatsiyasi
            var scheme = IsHttps ? "https" : "http";
            var testUrl = $"{scheme}://{Host}:{Port}";

            if (!Uri.TryCreate(testUrl, UriKind.Absolute, out _))
            {
                Error = "❌ Noto'g'ri URL formati";
                return;
            }

            // Vaqtincha URL ni yangilash (hali saqlamasdan)
            var originalUrl = apiConnection.Url;
            apiConnection.Url = testUrl;

            // Bog'lanishni tekshirish
            var tester = services.GetRequiredService<ConnectionTester>();
            var isConnected = await tester.TestAsync();

            if (isConnected)
            {
                Success = "✓ Server bilan aloqa muvaffaqiyatli!";
                apiConnection.IsConnected = true;
            }
            else
            {
                Error = "✗ Server bilan bog'lanib bo'lmadi";
                apiConnection.IsConnected = false;
                // Muvaffaqiyatsiz bo'lsa, eski URLni qaytarish
                apiConnection.Url = originalUrl;
            }

            SyncFromApiConnection();
        }
        catch (Exception ex)
        {
            Error = $"❌ Xatolik: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        try
        {
            Error = string.Empty;
            Success = string.Empty;
            Warning = string.Empty;
            IsLoading = true;

            // Sozlamalarni saqlash
            await apiConnection.SaveConnectionSettingsCommand.ExecuteAsync(null);

            // ApiConnectionViewModel dan xabarlarni olish
            if (!string.IsNullOrEmpty(apiConnection.Error))
            {
                Error = apiConnection.Error;
                return;
            }

            if (!string.IsNullOrEmpty(apiConnection.Success))
            {
                Success = apiConnection.Success;
            }

            // Muvaffaqiyatli saqlangandan keyin oynani yopish
            await Task.Delay(800); // Foydalanuvchiga xabarni ko'rsatish
            closeAction?.Invoke();
        }
        catch (Exception ex)
        {
            Error = $"Saqlashda xatolik: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}