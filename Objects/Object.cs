using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

public class SearchIndicator : ObservableObject { }
public partial class SearchResult:SearchIndicator
{
    [ObservableProperty]
    private string title;
    [ObservableProperty]
    private string type;
    [ObservableProperty]
    private string cover;
    public override string ToString()
    {
        return Title;
    }
}
public partial class SearchGroup : SearchIndicator
{
    [ObservableProperty]
    private string name;
}

public enum BouncyType:int
{
    Game=1,Anime=2,Movie=3
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
    private string name;

    [ObservableProperty]
    private string firstTag;

    [ObservableProperty]
    private string size;

    [ObservableProperty]
    private string rank;

    [ObservableProperty]
    private string code;
}

public class UpdateInfo
{
    public required string Remark { get; set; }
    public required string ClientVersion { get; set; }
    public required string DataBaseVersion { get; set; }
    public required string MD5 { get; set; }
    public bool Status { get; set; }
}
public class Data
{
    public int Id { get; set; }
    public string Name1 { get; set; }
    public string Name2 { get; set; }
    public string BH { get; set; }

    public string MageA { get; set; }
    public string BiaoQ {  get; set; }

    public string XingJ {  get; set; }
    public string RiQ {  get; set; }
    public string FXRiQ {  get; set; }
    public string RongL {  get; set; }
}
