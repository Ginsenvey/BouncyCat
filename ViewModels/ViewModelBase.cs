using BouncyCat.Objects;
using BouncyCat.Services;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BouncyCat.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _nav;

    [ObservableProperty]
    private ObservableCollection<NavigationItem> menuItems = new();

    [ObservableProperty]
    private ObservableCollection<NavigationItem> footerItems=new();

    [ObservableProperty]
    private NavigationItem? selectdItem;

    [RelayCommand]
    public void ItemInvoke(string tag)
    {
        foreach (var i in MenuItems)
        {
            if (i.Tag != tag)
            {
                i.IsSelected = false;
            }
            else
            {
                i.IsSelected = true;
                SelectdItem= i;
            }

        }
        foreach (var i in FooterItems)
        {
            if (i.Tag != tag)
            {
                i.IsSelected = false;
            }
            else
            {
                i.IsSelected = true;
                SelectdItem = i;
            }

        }
    }

    public MainViewModel(INavigationService nav) => _nav = nav;

    [RelayCommand]
    private void GoBack() => _nav.GoBack();
}