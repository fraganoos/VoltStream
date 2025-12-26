namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VoltStream.WPF.Commons;

public partial class CategoryViewModel : ViewModelBase
{
    public long Id { get; set; }
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private ObservableCollection<ProductViewModel> products = [];
}