namespace VoltStream.WPF.Commons;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons.Enums;
using VoltStream.WPF.Commons.Services;

public abstract partial class ViewModelBase : ObservableObject
{

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string? error;
    [ObservableProperty] private string? success;
    [ObservableProperty] private string? warning;
    [ObservableProperty] private string? info;
    [ObservableProperty] private bool isSelected;
    [ObservableProperty] private bool idEditing;

    partial void OnErrorChanged(string? value)
        => HandleMessage(value, NotificationType.Error, () => Error = null);

    partial void OnSuccessChanged(string? value)
        => HandleMessage(value, NotificationType.Success, () => Success = null);

    partial void OnWarningChanged(string? value)
        => HandleMessage(value, NotificationType.Warning, () => Warning = null);

    partial void OnInfoChanged(string? value)
        => HandleMessage(value, NotificationType.Info, () => Info = null);

    private static void HandleMessage(string? value, NotificationType type, Action clearAction)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        int durationSeconds = Math.Max(3, value.Length / 20 + 3);
        NotificationService.Show(value, type, durationSeconds);

        _ = Task.Run(async () =>
        {
            await Task.Delay(durationSeconds * 1000);
            clearAction();
        });
    }

    public void ClearMessages()
    {
        Error = Success = Warning = Info = null;
    }
}