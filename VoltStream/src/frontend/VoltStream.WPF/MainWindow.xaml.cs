namespace VoltStream.WPF;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoltStream.WPF.Commons.Animations;

public partial class MainWindow : Window
{
    private readonly MainViewModel vm;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        vm = new MainViewModel(serviceProvider);
        DataContext = vm;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (vm is not null)
        {
            vm.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsSidebarCollapsed))
        {
            if (vm.IsSidebarCollapsed)
                CollapseSidebar();
            else
                ExpandSidebar();
        }
    }

    private void CollapseSidebar()
    {
        var animation = new GridLengthAnimation
        {
            From = new GridLength(SidebarColumn.ActualWidth, GridUnitType.Pixel),
            To = new GridLength(60, GridUnitType.Pixel),
            Duration = new Duration(TimeSpan.FromMilliseconds(300))
        };
        SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);

        // Matnlarni yashirish
        foreach (var tb in FindVisualChildren<TextBlock>(Sidebar))
        {
            if (tb.Name != "LogoText")
                tb.Visibility = Visibility.Collapsed;
        }

        // StackPanel’larni markazga joylashtirish
        foreach (var sp in FindVisualChildren<StackPanel>(Sidebar))
        {
            if (sp.Orientation == Orientation.Horizontal)
                sp.HorizontalAlignment = HorizontalAlignment.Center;
        }

    }

    private void ExpandSidebar()
    {
        var animation = new GridLengthAnimation
        {
            From = new GridLength(SidebarColumn.ActualWidth, GridUnitType.Pixel),
            To = new GridLength(250, GridUnitType.Pixel),
            Duration = new Duration(TimeSpan.FromMilliseconds(300))
        };
        SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);

        // Matnlarni ko‘rsatish
        foreach (var tb in FindVisualChildren<TextBlock>(Sidebar))
        {
            tb.Visibility = Visibility.Visible;
        }

        // StackPanel’larni chapga qaytarish
        foreach (var sp in FindVisualChildren<StackPanel>(Sidebar))
        {
            if (sp.Orientation == Orientation.Horizontal)
                sp.HorizontalAlignment = HorizontalAlignment.Left;
        }

    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t)
                yield return t;

            foreach (var childOfChild in FindVisualChildren<T>(child))
                yield return childOfChild;
        }
    }
}
