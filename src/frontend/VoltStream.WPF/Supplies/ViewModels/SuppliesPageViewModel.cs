namespace VoltStream.WPF.Supplies.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Messages;
using VoltStream.WPF.Commons.ViewModels;

public partial class SuppliesPageViewModel : ViewModelBase
{
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly ISuppliesApi suppliesApi;
    private readonly IWarehouseStocksApi warehouseItemsApi;
    private readonly IMapper mapper;

    public SuppliesPageViewModel(IServiceProvider services)
    {
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        suppliesApi = services.GetRequiredService<ISuppliesApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseStocksApi>();
        mapper = services.GetRequiredService<IMapper>();

        SelectedDate = DateTime.Now;

        _ = LoadDataAsync();
    }

    [ObservableProperty] private DateTime selectedDate;
    [ObservableProperty] private ObservableCollection<CategoryViewModel> categories = [];
    [ObservableProperty] private CategoryViewModel? selectedCategory;

    [ObservableProperty] private string categoryText = string.Empty;

    [ObservableProperty] private ObservableCollection<ProductViewModel> products = [];
    [ObservableProperty] private ProductViewModel? selectedProduct;

    [ObservableProperty] private string productText = string.Empty;

    [ObservableProperty] private decimal? perRollCount;
    [ObservableProperty] private int? rollCount;
    [ObservableProperty] private decimal? unitPrice;
    [ObservableProperty] private decimal? discountRate;
    [ObservableProperty] private decimal? totalQuantity;
    [ObservableProperty] private string unit;

    [ObservableProperty] private ObservableCollection<SupplyViewModel> supplies = [];
    [ObservableProperty] private SupplyViewModel? selectedSupply;

    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private SupplyViewModel? editingItemBackup;
    private int _editingItemIndex = -1;

    #region Property Change Handlers

    partial void OnSelectedDateChanged(DateTime value)
    {
        _ = LoadSuppliesAsync();
    }

    partial void OnSelectedCategoryChanged(CategoryViewModel? value)
    {
        if (value is not null)
        {
            CategoryText = value.Name;
            _ = LoadProductsAsync(value.Id);
        }
    }

    partial void OnCategoryTextChanged(string? oldValue, string newValue)
    {
        var old = Categories.FirstOrDefault(c => string.Equals(c.Name, oldValue, StringComparison.OrdinalIgnoreCase) && c.Id < 1);
        if (old is not null)
            Categories.Remove(old);

        if (string.IsNullOrWhiteSpace(newValue)) return;

        if (SelectedCategory is not null && string.Equals(SelectedCategory.Name, newValue, StringComparison.OrdinalIgnoreCase))
            return;

        var existing = Categories.FirstOrDefault(c => string.Equals(c.Name, newValue, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            SelectedCategory = existing;
            return;
        }

        var result = MessageBox.Show($"'{newValue}' bu kategoriyalar ro'yxatida mavjud emas. Yangi kategoriya sifatida qo'shishni istaysizmi?",
                            "Tasdiqlash",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Categories.Add(new() { Name = newValue });
            SelectedCategory = null;

            return;
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new FocusControlMessage("Category"));
            CategoryText = string.Empty;
            SelectedCategory = null;
            return;
        }
    }

    partial void OnSelectedProductChanged(ProductViewModel? value)
    {
        if (value is not null)
        {
            ProductText = value.Name;
            Unit = value.Unit ?? "metr";
            _ = LoadProductDetails(value.Id);
        }
    }

    partial void OnProductTextChanged(string? oldValue, string newValue)
    {
        var old = Products.FirstOrDefault(c => string.Equals(c.Name, oldValue, StringComparison.OrdinalIgnoreCase) && c.Id < 1);
        if (old is not null)
            Products.Remove(old);

        if (string.IsNullOrWhiteSpace(newValue)) return;

        if (SelectedProduct is not null && string.Equals(SelectedProduct.Name, newValue, StringComparison.OrdinalIgnoreCase))
            return;

        var existing = Products.FirstOrDefault(c => string.Equals(c.Name, newValue, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            SelectedProduct = existing;
            return;
        }

        var result = MessageBox.Show($"'{newValue}' bu mahsulotlar ro'yxatida mavjud emas. Yangi mahsulot sifatida qo'shishni istaysizmi?",
                            "Tasdiqlash",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Products.Add(new() { Name = newValue });
            SelectedProduct = null;

            return;
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new FocusControlMessage("Product"));
            ProductText = string.Empty;
            SelectedProduct = null;
            return;
        }
    }

    partial void OnPerRollCountChanged(decimal? value) => CalculateTotal();
    partial void OnRollCountChanged(int? value) => CalculateTotal();

    #endregion Property Change Handlers

    #region Load Data

    private async Task LoadDataAsync()
    {
        await Task.WhenAll(
            LoadCategoriesAsync(),
            LoadSuppliesAsync()
        );
    }

    private async Task LoadCategoriesAsync()
    {
        var result = await categoriesApi.GetAllAsync().Handle();
        if (result.IsSuccess) Categories = mapper.Map<ObservableCollection<CategoryViewModel>>(result.Data);
        else Error = result.Message ?? "Kategoriyalarni yuklashda xatolik";
    }

    private async Task LoadProductsAsync(long? categoryId)
    {
        if (categoryId is null)
        {
            var result = await productsApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
            if (result.IsSuccess) Products = mapper.Map<ObservableCollection<ProductViewModel>>(result.Data);
            else Error = result.Message ?? "Mahsulotlarni yuklashda xatolik";
        }
        else
        {
            var result = await productsApi.GetAllByCategoryIdAsync(categoryId.Value).Handle(isLoading => IsLoading = isLoading);
            if (result.IsSuccess) Products = mapper.Map<ObservableCollection<ProductViewModel>>(result.Data);
            else Error = result.Message ?? "Mahsulotlarni yuklashda xatolik";
        }
    }

    private async Task LoadSuppliesAsync()
    {
        var filter = new FilteringRequest
        {
            Filters = new()
            {
                ["date"] = [$"{SelectedDate:yyyy-MM-dd}"],
                ["product"] = ["include:category"]
            },
            Descending = true
        };

        var result = await suppliesApi.Filter(filter).Handle(isLoading => IsLoading = isLoading);
        if (result.IsSuccess)
        {
            Supplies.Clear();
            foreach (var item in result.Data)
            {
                Supplies.Add(MapToViewModel(item));
            }
        }
        else Error = result.Message ?? "Ta'minotlarni yuklashda xatolik";
    }

    private async Task LoadProductDetails(long productId)
    {
        var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync().Handle();
        if (warehouseItems?.Data is not null)
        {
            var item = warehouseItems.Data.FirstOrDefault(x => x.ProductId == productId);
            if (item is not null)
            {
                UnitPrice = item.UnitPrice;
                DiscountRate = item.DiscountRate;
            }
        }
    }

    #endregion Load Data

    #region Commands

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(CategoryText) || string.IsNullOrWhiteSpace(ProductText))
        {
            MessageBox.Show("Kategoriya va Mahsulot nomi kiritilishi shart!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var request = new SupplyRequest
        {
            Id = IsEditing && EditingItemBackup is not null ? EditingItemBackup.Id : 0,
            Date = (SelectedDate.Date == DateTime.Today ? DateTime.Now : SelectedDate).ToUniversalTime(),
            CategoryId = SelectedCategory?.Id ?? 0,
            ProductId = SelectedProduct?.Id ?? 0,
            RollCount = RollCount ?? 0,
            Unit = Unit,
            LengthPerRoll = PerRollCount ?? 0,
            TotalLength = TotalQuantity ?? 0,
            ProductName = ProductText,
            CategoryName = CategoryText,
            UnitPrice = UnitPrice ?? 0,
            DiscountRate = DiscountRate ?? 0
        };

        var isSuccess = false;
        string errorMsg = "";

        if (IsEditing)
        {
            var result = await suppliesApi.UpdateSupplyAsync(request).Handle(isLoading => IsLoading = isLoading);
            if (result.IsSuccess) isSuccess = true;
            else errorMsg = result.Message ?? "Mahsulotni yangilashda xatolik yuz berdi.";
        }
        else
        {
            var result = await suppliesApi.CreateSupplyAsync(request).Handle(isLoading => IsLoading = isLoading);
            if (result.IsSuccess) isSuccess = true;
            else errorMsg = result.Message ?? "Mahsulot qo'shishda xatolik yuz berdi.";
        }

        if (isSuccess)
        {
            ClearForm();
            if (IsEditing)
            {
                IsEditing = false;
                EditingItemBackup = null;
                _editingItemIndex = -1;
            }
            await LoadDataAsync();
        }
        else
        {
            MessageBox.Show(errorMsg, "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task EditItem(SupplyViewModel item)
    {
        if (item is null) return;

        if (IsFormDirty())
        {
            var result = MessageBox.Show(
                "Formada saqlanmagan ma'lumotlar mavjud. Agar davom ettirsangiz, ular o'chib ketadi. Davom ettirishni istaysizmi?",
                "Diqqat",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
                return;
        }

        if (_editingItemIndex > -1 && EditingItemBackup is not null)
            Supplies.Insert(_editingItemIndex, EditingItemBackup);

        IsEditing = true;
        EditingItemBackup = item;
        _editingItemIndex = Supplies.IndexOf(item);

        Supplies.Remove(item);

        FillFormFromItem(item);
    }

    [RelayCommand]
    private void CancelEdit()
    {
        if (IsEditing && EditingItemBackup is not null)
        {
            if (_editingItemIndex >= 0 && _editingItemIndex <= Supplies.Count)
                Supplies.Insert(_editingItemIndex, EditingItemBackup);
            else
                Supplies.Add(EditingItemBackup);

            ClearForm();
            IsEditing = false;
            EditingItemBackup = null;
            _editingItemIndex = -1;
        }
    }

    [RelayCommand]
    private async Task DeleteItem(SupplyViewModel item)
    {
        if (item is null) return;

        var result = MessageBox.Show("Haqiqatan ham o'chirmoqchimisiz?", "O'chirish", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            var ViewModel = await suppliesApi.DeleteSupplyAsync(item.Id).Handle();
            if (ViewModel.IsSuccess)
            {
                Supplies.Remove(item);
            }
            else
            {
                MessageBox.Show($"O'chirishda xatolik: {ViewModel.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #endregion Commands

    #region Helper Methods

    private bool IsFormDirty()
    {
        if (!string.IsNullOrWhiteSpace(CategoryText) ||
            !string.IsNullOrWhiteSpace(ProductText) ||
            RollCount > 0 ||
            PerRollCount > 0 ||
            UnitPrice > 0 ||
            DiscountRate > 0)
        {
            return true;
        }
        return false;
    }

    private async void FillFormFromItem(SupplyViewModel item)
    {
        if (SelectedCategory?.Id != item.CategoryId)
        {
            var cat = Categories.FirstOrDefault(c => c.Id == item.CategoryId);
            SelectedCategory = cat;
            CategoryText = cat?.Name ?? item.CategoryName;

            await LoadProductsAsync(item.CategoryId);
        }

        SelectedProduct = item.Product;
        ProductText = item.ProductName;
        PerRollCount = item.LengthPerRoll;
        RollCount = item.RollCount;
        UnitPrice = item.UnitPrice;
        DiscountRate = item.DiscountRate;
        CalculateTotal();
    }

    private void ClearForm()
    {
        SelectedCategory = null;
        CategoryText = string.Empty;

        SelectedProduct = null;
        ProductText = string.Empty;

        PerRollCount = null;
        RollCount = null;
        TotalQuantity = null;
        UnitPrice = null;
        DiscountRate = null;
        Unit = null!;
    }

    private SupplyViewModel MapToViewModel(SupplyResponse viewModel)
    {
        return new SupplyViewModel
        {
            Id = viewModel.Id,
            Date = viewModel.Date,
            CategoryId = viewModel.Product?.CategoryId ?? 0,
            CategoryName = viewModel.Product?.Category?.Name ?? "",
            ProductId = viewModel.ProductId,
            ProductName = viewModel.Product?.Name ?? "",
            RollCount = viewModel.RollCount,
            LengthPerRoll = viewModel.LengthPerRoll,
            TotalLength = viewModel.TotalLength,
            UnitPrice = viewModel.UnitPrice,
            DiscountRate = viewModel.DiscountRate,
            Unit = viewModel.Product?.Unit ?? "metr"
        };
    }

    private void CalculateTotal()
    {
        TotalQuantity = PerRollCount * RollCount;
    }

    #endregion Helper Methods
}
