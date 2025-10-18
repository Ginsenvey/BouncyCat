using BouncyCat.Messengers;
using BouncyCat.Objects;
using BouncyCat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using SqlSugar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace BouncyCat.ViewModels;

public sealed partial class DiscoverViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<GameSection> sections = new();

    [ObservableProperty]
    private ObservableCollection<Game> games = new();

    [ObservableProperty]
    private Visibility splitterVisibility = Visibility.Visible;
    [RelayCommand]
    public void Expanding(bool value)
    {
        SplitterVisibility=value?Visibility.Collapsed:Visibility.Visible;
    }

    public DiscoverViewModel()
    {
        InitializeSampleData();
        
    }
    private void InitializeSampleData()
    {
        for (int i = 0; i < 16; i++)
        {
            Sections.Add(new GameSection { Name="像素",Count = 10038});
        }
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "Data Source=C:\\Users\\Ansherly\\Downloads\\WenJian.db", // 数据库文件路径
            DbType = DbType.Sqlite, // 数据库类型
            IsAutoCloseConnection = true // 是否自动关闭连接
        });
        List<Data> userList = db.Queryable<Data>().ToList();
        foreach(Data data in userList)
        {
            Games.Add(new Game { Name = data.Name1, Url = data.MageA });
        }
    }


}