using BouncyCat.Objects;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using BouncyCat.Views;
namespace BouncyCat.ViewModels;

public sealed partial class DiscoverViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<GameSection> sections = new();

    [ObservableProperty]
    private ObservableCollection<Game> games = new();

    [ObservableProperty]
    private bool _isHided;

    // 第一列的宽度属性
    [ObservableProperty]
    private GridLength firstColumnWidth = new(320);

    partial void OnIsHidedChanged(bool value)
    {
        FirstColumnWidth = value ? new GridLength(0) : new GridLength(320);

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
        for (int i = 0; i < 16; i++)
        {
            Games.Add(new Game { Url = "像素", Id = 10038 });
        }

    }


}