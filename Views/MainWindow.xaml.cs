using BouncyCat.Objects;
using BouncyCat.Services;
using BouncyCat.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using FluentIcons.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace BouncyCat.Views
{
    
    public sealed partial class MainWindow : Window
    {
        private readonly NavigationService? nav;
        public MainViewModel ViewModel { get; }
        public MainWindow()
        {
            InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(GridTitleBar);
            this.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            ViewModel = App.Services.GetRequiredService<MainViewModel>();
            nav = App.Services.GetRequiredService<INavigationService>() as NavigationService;
            nav?.SetFrame(RootFrame);
            AddNavigationItems();
            nav?.NavigateTo(typeof(Discover));
            ViewModel.InitializeData();
            
        }
        
        private void AddNavigationItems()
        {
            ViewModel.MenuItems.Add(new NavigationItem { Name = "主页",IconSymbol=FluentIcons.Common.Symbol.Home,Tag="home" });
            ViewModel.MenuItems.Add(new NavigationItem { Name = "对战", IconSymbol = FluentIcons.Common.Symbol.People, Tag = "section" });
            ViewModel.MenuItems.Add(new NavigationItem { Name = "详情", IconSymbol = FluentIcons.Common.Symbol.ContactCardGroup, Tag = "detail" });
            ViewModel.MenuItems.Add(new NavigationItem { Name = "环境", IconSymbol = FluentIcons.Common.Symbol.HardDrive, Tag = "environment" });
            ViewModel.MenuItems.Add(new NavigationItem { Name = "教程", IconSymbol = FluentIcons.Common.Symbol.WebAsset, Tag = "guide" });
            ViewModel.MenuItems.Add(new NavigationItem { Name = "问答", IconSymbol = FluentIcons.Common.Symbol.ChatBubblesQuestion, Tag = "question" });
            ViewModel.FooterItems.Add(new NavigationItem { Name = "设置", IconSymbol = FluentIcons.Common.Symbol.StarSettings, Tag = "setting" });
        }

        private void Navi_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer?.Tag is string tag)
            {
                ViewModel.ItemInvoke(tag);
                var item = args.InvokedItem as NavigationViewItem;
                switch (tag)
                {
                    case "home":
                        break;
                    case "section":
                        nav?.NavigateTo(typeof(Discover));
                        break;
                    case "setting":
                        nav?.NavigateTo(typeof(Settings));
                        break;
                }
            }
                
           

        }

        private async void RootSerchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var Text = sender.Text.ToLower();
                WeakReferenceMessenger.Default.Send(Text);
                if(RootFrame.Content is Discover discover)
                {
                    return;
                }
                else
                {
                    await ViewModel.ExecuteSearch(Text);
                }

            }
        }
    }

    public partial class BooltoVariantConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool flag)
            {
                return flag ? IconVariant.Filled : IconVariant.Regular;
            }
            else
            {
                return IconVariant.Regular;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SearchResultSelector : DataTemplateSelector
    {
        public  DataTemplate GroupTemplate { get; set; }
        public  DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is SearchGroup g)
            {
                return GroupTemplate;
            }
            else
            {
                return ItemTemplate;
            }
        }
    }


}
