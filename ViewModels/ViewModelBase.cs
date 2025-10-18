using BouncyCat.Objects;
using BouncyCat.Services;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace BouncyCat.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    public MainViewModel(INavigationService nav, IUpdateService updateService)
    {
        _nav = nav;
        _updateService = updateService;
    }
    public static HttpClient client=new();
    private readonly INavigationService _nav;
    private readonly IUpdateService _updateService;
    [ObservableProperty]
    private ObservableCollection<NavigationItem> menuItems = new();

    [ObservableProperty]
    private ObservableCollection<NavigationItem> footerItems=new();

    [ObservableProperty]
    private NavigationItem? selectdItem;

    [ObservableProperty]
    private bool isLoading=true;

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

    [RelayCommand]
    public async Task InitializeData()
    {
        //string r=await _updateService.UpdateAsync();
        //System.Diagnostics.Debug.WriteLine(r);
        await Task.Delay(1500);
        IsLoading = false;
    }

    

    [RelayCommand]
    private void GoBack() => _nav.GoBack();
}