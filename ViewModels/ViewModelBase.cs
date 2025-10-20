using BouncyCat.Objects;
using BouncyCat.Services;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SqlSugar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BouncyCat.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    public MainViewModel(INavigationService nav, IUpdateService updateService)
    {
        _nav = nav;
        _updateService = updateService;
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "Data Source=C:\\Users\\Ansherly\\Downloads\\WenJian.db", // 数据库文件路径
            DbType = DbType.Sqlite, // 数据库类型
            IsAutoCloseConnection = true // 是否自动关闭连接
        });
        GameList = db.Queryable<Data>().ToList();
    }
    public static HttpClient client=new();
    private readonly INavigationService _nav;
    private readonly IUpdateService _updateService;
    public List<Data> GameList { get; set; }
    [ObservableProperty]
    private ObservableCollection<NavigationItem> menuItems = new();

    [ObservableProperty]
    private ObservableCollection<NavigationItem> footerItems=new();

    [ObservableProperty]
    private NavigationItem? selectdItem;

    [ObservableProperty]
    private ObservableCollection<SearchIndicator> results=new() ;
    

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
    public void ExecuteSearch(string text)
    {
        Results.Clear();
        var groups = GameList.Where(g=>g.Name1.Contains(text)).GroupBy(g => g.BiaoQ);
        foreach (var group in groups)
        {
            Results.Add(new SearchGroup { Name = group.Key });
            foreach (var result in group)
            {
                Results.Add(new SearchResult { Title = result.Name1, Type = result.BiaoQ,Cover=result.MageA });
            }
        }
        
    }

    [RelayCommand]
    private void GoBack() => _nav.GoBack();
}