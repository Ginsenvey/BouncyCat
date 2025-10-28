using BouncyCat.Messengers;
using BouncyCat.Objects;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System; // DispatcherQueue

namespace BouncyCat.ViewModels;

public sealed partial class DiscoverViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<GameSection> sections = new();

    [ObservableProperty]
    private ObservableCollection<Game> games = new();

    [ObservableProperty]
    private Visibility splitterVisibility = Visibility.Visible;

    private List<Data> allData = new(); // 缓存从数据库读取的全部数据

    [RelayCommand]
    public void Expanding(bool value)
    {
        SplitterVisibility = value ? Visibility.Collapsed : Visibility.Visible;
    }

    public DiscoverViewModel()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        for (int i = 0; i < 16; i++)
        {
            Sections.Add(new GameSection { Name = "像素", Count = 10038 });
        }
        string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string path = System.IO.Path.Combine(localAppDataPath, "BouncyCat", "Data.db");
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = $"Data Source={path}", // 数据库文件路径
            DbType = DbType.Sqlite, // 数据库类型
            IsAutoCloseConnection = true // 是否自动关闭连接
        });
        allData = db.Queryable<Data>().ToList();

        LoadPage(1);
    }

    // 返回指定页的数据（page 从 1 开始），每页默认 20 条
    public IEnumerable<Game> GetPage(int page, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (allData == null || allData.Count == 0) return Enumerable.Empty<Game>();

        return allData
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new Game { Name = d.Name1, Url = d.MageA })
            .ToList();
    }

    
    [RelayCommand]
    public void LoadPage(int page)
    {
        var items = GetPage(page);
        foreach (var g in items)
        {
            Games.Add(g);
        }
    }

    
}