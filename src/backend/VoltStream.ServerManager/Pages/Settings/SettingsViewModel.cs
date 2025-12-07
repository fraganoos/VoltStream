namespace VoltStream.ServerManager.Pages.Settings;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using VoltStream.ServerManager.Models;
using VoltStream.ServerManager.Services;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private AppSettings settings = SettingsService.Load();

    [RelayCommand]
    private void Save()
    {
        SettingsService.Save(Settings);
        MessageBox.Show("Settings saved successfully!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}