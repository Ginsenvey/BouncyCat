using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace BouncyCat.Objects;



/// 设定[Observable]字段不得带有required.

public partial class NavigationItem : ObservableObject
{
    
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private FluentIcons.Common.Symbol iconSymbol;

    [ObservableProperty]
    private string tag;

    [ObservableProperty]
    public bool isSelected;

    
}



public partial class GameSection : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private int count;

    [ObservableProperty]
    private bool pinned;
   
}

public partial class Game : ObservableObject
{
    [ObservableProperty]
    private string url;

    [ObservableProperty]
    private int id;

}

public class UpdateInfo
{
    public required string Remark { get; set; }
    public required string ClientVersion { get; set; }
    public required string DataBaseVersion { get; set; }
    public required string MD5 { get; set; }
    public bool Status { get; set; }
}
