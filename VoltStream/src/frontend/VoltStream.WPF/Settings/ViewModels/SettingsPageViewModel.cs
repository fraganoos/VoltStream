namespace VoltStream.WPF.Settings.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IMapper mapper;
    private readonly IServiceProvider services;

    public SettingsPageViewModel(IServiceProvider services)
    {
        this.services = services;
        mapper = services.GetRequiredService<IMapper>();
        apiConnection = services.GetRequiredService<ApiConnectionViewModel>();

        _ = LoadData();
    }

    [ObservableProperty] private ApiConnectionViewModel apiConnection;
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];

    #region Commands

    [RelayCommand]
    private void AddCurrency()
    {
        Currencies.Add(new CurrencyViewModel());
    }

    [RelayCommand]
    private void RemoveCurrency(CurrencyViewModel currency)
    {
        Currencies.Remove(currency);
    }

    [RelayCommand]
    private async Task SaveCurrencies()
    {
        if (Currencies is null || Currencies.Count == 0)
        {
            Warning = "Saqlash uchun valyuta yo‘q";
            return;
        }

        var client = services.GetRequiredService<ICurrenciesApi>();
        var dtoList = mapper.Map<List<CurrencyRequest>>(Currencies);

        var response = await client.SaveAllAsync(dtoList)
            .Handle(isLoading => IsSelected = isLoading);

        if (response.IsSuccess) Success = "O'zgarishlar muvaffaqiyatli saqlandi";
        else Error = response.Message ?? "Valyutalarni saqlashda xatolik";
    }

    #endregion Commands

    #region Load Data

    private async Task LoadData()
    {
        await LoadCurrencies();
    }

    private async Task LoadCurrencies()
    {
        var client = services.GetRequiredService<ICurrenciesApi>();
        var response = await client.GetAllAsync().Handle();

        if (response.IsSuccess)
            Currencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data);
        else Error = response.Message ?? "Valyutalarni yuklashda xatolik";
    }

    #endregion Load Data
}
