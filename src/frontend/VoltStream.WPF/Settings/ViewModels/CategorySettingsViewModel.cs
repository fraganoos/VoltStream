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

public partial class CategorySettingsViewModel : ViewModelBase
{
    private readonly ICategoriesApi categoriesApi;

    public CategorySettingsViewModel(IServiceProvider services)
    {
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        _ = LoadCategories();
    }

    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];
    [ObservableProperty] private CategoryResponse? selectedCategory;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private bool isEditing;

    public async Task LoadCategories()
    {
        var response = await categoriesApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess) Categories = new ObservableCollection<CategoryResponse>(response.Data);
        else Error = response.Message ?? "Kategoriyalarni yuklashda xatolik!";
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        if (IsEditing && SelectedCategory != null)
        {
            var response = await categoriesApi.UpdateAsync(new CategoryRequest { Id = SelectedCategory.Id, Name = Name })
                .Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Kategoriya ma'lumotlari muvaffaqiyatli yangilandi!";
                await LoadCategories();
                Cancel();
            }
            else Error = response.Message ?? "Kategoriya ma'lumotlarini yangilashda xatolik!";
        }
        else
        {
            var response = await categoriesApi.CreateAsync(new CategoryRequest { Name = Name })
                .Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Kategoriya muvaffaqiyatli yaratildi!";
                await LoadCategories();
                Cancel();
            }
            else Error = response.Message ?? "Kategoriya yaratishda xatolik!";
        }
    }

    [RelayCommand]
    private void Edit(CategoryResponse category)
    {
        SelectedCategory = category;
        Name = category.Name;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task Delete(CategoryResponse category)
    {
        if (MessageBox.Show($"{category.Name} kategoriyani o'chirishni tasdiqlaysizmi?", "O'chirish", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        var response = await categoriesApi.DeleteAsync(category.Id).Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
        {
            Success = "Kategoriya muvaffaqiyatli o'chirildi!";
            await LoadCategories();
            if (SelectedCategory == category) Cancel();
        }
        else Error = response.Message ?? "Kategoriyani o'chirishda xatolik!";
    }

    [RelayCommand]
    private void Cancel()
    {
        SelectedCategory = null;
        Name = string.Empty;
        IsEditing = false;
    }
}
