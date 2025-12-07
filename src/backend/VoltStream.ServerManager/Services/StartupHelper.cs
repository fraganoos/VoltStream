namespace VoltStream.ServerManager.Services;

using Microsoft.Win32;

public static class StartupHelper
{
    private const string AppName = "VoltStream ServerManager";

    public static void RegisterInStartup()
    {
        string exePath = Environment.ProcessPath!;
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;
        key.SetValue(AppName, $"\"{exePath}\"");
    }

    public static void UnregisterFromStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;
        key.DeleteValue(AppName, false);
    }
}