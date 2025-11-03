using BouncyCat.Messengers;
using BouncyCat.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BouncyCat.Views
{
    
    public sealed partial class Discover : Page
    {
        public DiscoverViewModel ViewModel { get; }
        public bool Expanded = true;

        public Discover()
        {
            InitializeComponent();
            ViewModel = App.Services.GetRequiredService<DiscoverViewModel>();
            WeakReferenceMessenger.Default.Register<string>(this, (recipient, message) => {
                ViewModel.ExecuteSearchByName(message);
            });
        }
       

        private void Btn_Click(object sender, EventArgs e)
        {
            if (Expanded)
            {
                Title.Visibility = Visibility.Collapsed;
                SectionArea.Visibility = Visibility.Collapsed;
                LeftColumn.MinWidth = 0;
                LeftColumn.Width = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                Title.Visibility = Visibility.Visible;
                SectionArea.Visibility = Visibility.Visible;
                LeftColumn.Width = new GridLength(2, GridUnitType.Star);
            }
            ViewModel.Expanding(Expanded);
            Expanded = !Expanded;
        }
        public int history;
        private void GameList_Loaded(object sender, RoutedEventArgs e)
        {
            GameList.ElementPrepared += (s, args) =>
            {
                if (GameList.ItemsSource != null)
                {
                    int currentIndex = args.Index;
                    if (currentIndex > 0 && (currentIndex + 1) % 20 == 0 && currentIndex > history)
                    {
                        history = currentIndex;
                        int page = (currentIndex + 1) / 20 + 1; // 正确的页码计算
                        var dq = this.DispatcherQueue;
                        // 延后到消息队列中执行，避免在布局期间修改集合
                        dq?.TryEnqueue(() => ViewModel.LoadPage(page));
                    }
                }
            };
        }

        private void RangeSelector_ValueChanged(object sender, CommunityToolkit.WinUI.Controls.RangeChangedEventArgs e)
        {
            ViewModel.ApplyFilters(1);
        }

        private void SectionPicker_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateSectionPicker();
        }

        private void SectionPicker_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel?.UpdateSectionPicker();
        }

        private void ContentCard_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenInfoPane();
        }

        private void OrderController_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ApplyFilters(1);
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var i = sender as Image;
            if (i != null)
            {
                i.Source = new BitmapImage(new Uri("ms-appx:///Assets/fail.png"));
            }
        }
    }
}