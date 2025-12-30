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

    private async Task LoadCategories()
    {
        var response = await categoriesApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
        {
            Categories = new ObservableCollection<CategoryResponse>(response.Data);
        }
        else
        {
            Error = response.Message ?? "Error loading categories";
        }
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
                Success = "Saved successfully";
                await LoadCategories();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Error updating category";
            }
        }
        else
        {
            var response = await categoriesApi.CreateAsync(new CategoryRequest { Name = Name })
                .Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Created successfully";
                await LoadCategories();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Error creating category";
            }
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
        if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        var response = await categoriesApi.DeleteAsync(category.Id)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Deleted successfully";
            await LoadCategories();
            if (SelectedCategory == category) Cancel();
        }
        else
        {
            Error = response.Message ?? "Error deleting category";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        SelectedCategory = null;
        Name = string.Empty;
        IsEditing = false;
    }
}
