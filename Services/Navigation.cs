using Microsoft.UI.Xaml.Controls;
using System;

namespace BouncyCat.Services;

public interface INavigationService
{
    void SetFrame(Frame frame);
    void NavigateTo(Type pageType, object? param = null);
    void GoBack();
    bool CanGoBack { get; }
}

public class NavigationService : INavigationService
{
    private Frame? _frame;

    public void SetFrame(Frame frame) => _frame = frame;

    public void NavigateTo(Type pageType, object? param = null)
        => _frame?.Navigate(pageType, param);

    public void GoBack()
    {
        if (_frame?.CanGoBack == true) _frame.GoBack();
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;
}