namespace VoltStream.WPF.Turnovers.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using VoltStream.WPF.Commons;

public partial class TurnoversPageViewModel : ViewModelBase
{
    private readonly IServiceProvider services;

    public TurnoversPageViewModel(IServiceProvider services)
    {
        this.services = services;
        _ = LoadInitialDataAsync();

    }

    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];

    private async Task LoadInitialDataAsync()
    {
        await LoadCustomersAsync();
    }

    public async Task LoadCustomersAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICustomersApi>().GetAllAsync().Handle(isLoading=>IsLoading=isLoading);
            var mapper = services.GetRequiredService<IMapper>();
            if (response.IsSuccess)
                Customers = mapper.Map<ObservableCollection<CustomerResponse>>(response.Data!);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlar yuklanmadi: {ex.Message}");
        }
    }

}
