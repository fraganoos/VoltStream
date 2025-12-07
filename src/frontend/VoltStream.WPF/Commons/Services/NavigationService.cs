namespace VoltStream.WPF.Commons.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly Stack<object> navigationStack = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    private object? currentView;

    public bool CanGoBack => navigationStack.Count > 0;

    public void Navigate(object view)
    {
        if (CurrentView is not null)
        {
            navigationStack.Push(CurrentView);
        }

        CurrentView = view;
    }

    public void GoBack()
    {
        if (navigationStack.Count > 0)
        {
            CurrentView = navigationStack.Pop();
        }
    }
}
