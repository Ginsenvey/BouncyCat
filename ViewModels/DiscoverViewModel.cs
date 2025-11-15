using BouncyCat.Helpers;
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
using Windows.ApplicationModel.Search;
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

    [ObservableProperty]
    private int rangeStart = 0;

    [ObservableProperty]
    private int rangeEnd = 5;

    [ObservableProperty]
    private int order = 0;

    //存储分区筛选状态
    private List<string> SelectedSections = new();

    private string SearchText = "";

    private List<Data> AllData; // 缓存从数据库读取的全部数据
    public List<Data> FilteredData;

    [ObservableProperty]
    public bool isPaneOpen = false;

    [RelayCommand]
    public void Expanding(bool value)
    {
        SplitterVisibility = value ? Visibility.Collapsed : Visibility.Visible;
    }

    public DiscoverViewModel()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string path = System.IO.Path.Combine(localAppDataPath, "BouncyCat", "Data.db");
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = $"Data Source={path}", // 数据库文件路径
            DbType = DbType.Sqlite, 
            IsAutoCloseConnection = true 
        });
        AllData = db.Queryable<Data>().ToList();//全局缓存
        FilteredData=AllData.DeepCopy();//当前筛选条件的候选对象
        LoadSection();
        LoadPage(1);
    }
    public void LoadSection()
    {
        var section_info = AllData.Where(d => !string.IsNullOrEmpty(d.BiaoQ)).SelectMany(d=>d.BiaoQ.Split(".", StringSplitOptions.RemoveEmptyEntries)).GroupBy(tag => tag)
        .Select(g => new GameSection {  Name = g.Key, Count = g.Count(),Pinned=false })
        .OrderByDescending(x => x.Count)
        .ThenBy(x => x.Name);
        foreach (var section in section_info)
        {
            Sections.Add(section);
        }
    }
    // 返回指定页的数据（page 从 1 开始），每页默认 20 条;均基于已筛选的数据;
    public IEnumerable<Game> GetPage(int page, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (FilteredData == null || FilteredData.Count == 0) return Enumerable.Empty<Game>();

        return FilteredData
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new Game { Name = d.Name1, Url = d.MageA, FirstTag = d.BiaoQ.Split(".")[0],Size="12GB",Rank=d.XingJ,Code=d.BH })
            .ToList();
    }
    [RelayCommand]
    public void ExecuteSearchByName(string text)
    {
        SearchText = text;
        ApplyFilters(1);
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
    //FilterType是优化参数
    public void ApplyFilters(int FilterType)
    {
        // 基于内存中的全部数据进行筛选
        var filtered = AllData.AsEnumerable();


        if (SelectedSections.Count>0)
        {
            filtered = filtered.Where(d =>{
                var tags = d.BiaoQ.Split(".", StringSplitOptions.RemoveEmptyEntries).ToList();
                return tags.Intersect(SelectedSections).Any();
            });
        }


        if (!string.IsNullOrWhiteSpace(SearchText)&&(!string.IsNullOrEmpty(SearchText)))
        {
            filtered = filtered.Where(x =>
                x.Name1.Contains(SearchText, StringComparison.OrdinalIgnoreCase)||x.Name2.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (FilterType==1)
        {
            filtered = filtered.Where(x => {
                double rank = x.XingJ.ToDoubleSafe();
                return (rank <= RangeEnd&&rank>=RangeStart);
            });
        }
        switch (Order)
        {
            case 0:
                break;
            case 1:
                filtered = filtered.OrderBy(x => x.RiQ.ToIntSafe());
                break;
            case 2:
                filtered = filtered.OrderBy(x => x.FXRiQ.ToIntSafe());
                break;
            case 3:
                filtered = filtered.OrderBy(x => x.RongL.StringToSize());
                break;
        }
        // 更新显示数据
        UpdateFilteredCollection(filtered.ToList());
    }
    //更新搜索缓存，并从中取出数据给UI
    private void UpdateFilteredCollection(List<Data> NewData)
    {
        // 清空现有数据
        Games.Clear();
        FilteredData.Clear();
        FilteredData.AddRange(NewData);
        foreach (var d in FilteredData)
        {
            Games.Add(new Game {Name = d.Name1, Url = d.MageA, FirstTag = d.BiaoQ.Split(".")[0], Size = d.RongL, Rank = d.XingJ, Code = d.BH });
        }

    }
    //更新所选分区的缓存列表。
    public void UpdateSectionPicker()
    {   
        List<string> NewSelectedSections=Sections.Where(s=>s.Pinned).Select(s=>s.Name).ToList();
        SelectedSections.Clear();
        SelectedSections.AddRange(NewSelectedSections);
        ApplyFilters(1);
    }
    [RelayCommand]
    public void OpenInfoPane()=>IsPaneOpen=true;
   
}