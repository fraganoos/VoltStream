namespace VoltStream.WPF.Settings.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class CustomerSettingsViewModel : ViewModelBase
{
    private readonly ICustomersApi customersApi;

    public CustomerSettingsViewModel(IServiceProvider services)
    {
        customersApi = services.GetRequiredService<ICustomersApi>();
        _ = LoadCustomers();
    }

    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];
    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private bool isEditing;

    public async Task LoadCustomers()
    {
        var response = await customersApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
            Customers = new ObservableCollection<CustomerResponse>(response.Data);
        else
            Error = response.Message ?? "Error loading customers";
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        if (IsEditing && SelectedCustomer != null)
        {
            var request = new CustomerRequest
            {
                Id = SelectedCustomer.Id,
                Name = Name,
                Phone = Phone,
                Address = Address,
                Description = Description
            };

            var response = await customersApi.UpdateAsync(request).Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Saved successfully";
                await LoadCustomers();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Error updating customer";
            }
        }
        else
        {
            var request = new CustomerRequest
            {
                Name = Name,
                Phone = Phone,
                Address = Address,
                Description = Description
            };

            var response = await customersApi.CreateAsync(request).Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Created successfully";
                await LoadCustomers();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Error creating customer";
            }
        }
    }

    [RelayCommand]
    private void Edit(CustomerResponse customer)
    {
        SelectedCustomer = customer;
        Name = customer.Name;
        Phone = customer.Phone;
        Address = customer.Address;
        Description = customer.Description;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task Delete(CustomerResponse customer)
    {
        if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        var response = await customersApi.DeleteAsync(customer.Id).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Deleted successfully";
            await LoadCustomers();
            if (SelectedCustomer == customer) Cancel();
        }
        else
        {
            Error = response.Message ?? "Error deleting customer";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        SelectedCustomer = null;
        Name = string.Empty;
        Phone = string.Empty;
        Address = string.Empty;
        Description = string.Empty;
        IsEditing = false;
    }
}
