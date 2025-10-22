namespace VoltStream.WPF.Settings.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class SettingsPageViewModel(IServiceProvider services) : ViewModelBase
{
    [ObservableProperty] private ApiConnectionViewModel apiConnection = services.GetRequiredService<ApiConnectionViewModel>();
}
