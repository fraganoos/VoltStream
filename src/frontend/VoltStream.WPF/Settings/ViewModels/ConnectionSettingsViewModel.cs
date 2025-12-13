namespace VoltStream.WPF.Settings.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class ConnectionSettingsViewModel : ViewModelBase
{
    private Action? closeAction;

    public ConnectionSettingsViewModel(IServiceProvider services)
    {
        ApiConnection = services.GetRequiredService<ApiConnectionViewModel>();
    }

    [ObservableProperty] private ApiConnectionViewModel apiConnection;

    #region Commands

    [RelayCommand]
    private async Task TestConnection()
    {
        await ApiConnection.Save();

        if (ApiConnection.IsConnected)
            Success = "✓ Server bilan aloqa muvaffaqiyatli!";
        else Error = "✗ Server bilan bog'lanib bo'lmadi";
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        IsLoading = true;

        closeAction?.Invoke();
        await ApiConnection.Save();

        IsLoading = false;
    }

    #endregion Commands

    public void SetCloseAction(Action closeAction)
    {
        this.closeAction = closeAction;
    }
}