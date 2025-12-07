namespace VoltStream.WPF.Commons.Services;

public interface INavigationService
{
    object? CurrentView { get; }
    void Navigate(object view);
    void GoBack();
    bool CanGoBack { get; }
}
