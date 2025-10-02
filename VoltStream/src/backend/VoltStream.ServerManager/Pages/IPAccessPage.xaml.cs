namespace VoltStream.ServerManager.Pages;

using System.Windows;
using System.Windows.Controls;
using VoltStream.ServerManager.Api;
using VoltStream.ServerManager.Models;

public partial class IPAccessPage : Page
{
    private readonly IAllowedClientsApi allowedClientsApi;

    public IPAccessPage(IAllowedClientsApi allowedClientsApi)
    {
        InitializeComponent();
        this.allowedClientsApi = allowedClientsApi;
        Loaded += IPAccessPage_Loaded;
    }

    private async void IPAccessPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var response = await allowedClientsApi.GetAllAsync();
            IpDataGrid.ItemsSource = response.Data;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void IpDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is AllowedClientDto editedRow)
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    await allowedClientsApi.UpdateAsync(editedRow);
                    MessageBox.Show("Updated successfully!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Update xatolik: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
