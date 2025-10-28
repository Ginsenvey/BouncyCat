using BouncyCat.Objects;
using BouncyCat.Services;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SqlSugar;
using System;
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
        string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string path = System.IO.Path.Combine(localAppDataPath, "BouncyCat", "Data.db");
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = $"Data Source={path}", // 数据库文件路径
            DbType = DbType.Sqlite, // 数据库类型
            IsAutoCloseConnection = true // 是否自动关闭连接
        });
        GameList=db.Queryable<Data>().ToList();
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

    [ObservableProperty]
    private string ex = "无异常";

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
    public async Task ExecuteSearch(string text)
    {
        try
        {
            Results.Clear();

            if (string.IsNullOrWhiteSpace(text))
                return;

            if (GameList == null)
            {
                Ex = "数据未加载";
                return;
            }

            // 在后台线程执行查询和分组，减少 UI 线程压力
            var groups = await Task.Run(() =>
            {
                return GameList
                    .Where(g => !string.IsNullOrEmpty(g.Name1) 
                                && g.Name1.Contains(text, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(g => g.BiaoQ)
                    .ToList();
            });

            foreach (var group in groups)
            {
                Results.Add(new SearchGroup { Name = group.Key ?? string.Empty });
                foreach (var result in group)
                {
                    Results.Add(new SearchResult
                    {
                        Title = result.Name1 ?? string.Empty,
                        Type = result.BiaoQ ?? string.Empty,
                        Cover = result.MageA ?? string.Empty
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Ex = ex.Message;
        }
        
    }

    [RelayCommand]
    private void GoBack() => _nav.GoBack();
}