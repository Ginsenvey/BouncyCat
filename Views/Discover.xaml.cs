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
using Microsoft.UI.Xaml.Shapes;
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
        private void GamesItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            var container = args.Element as Grid;
            if (container == null) return;
            var item = ViewModel.Games[args.Index];
            if (item == null || string.IsNullOrEmpty(item.Url)) return;

            var loadingStateGrid = container.FindName("LoadingStateGrid") as Grid;
            var textContentPanel = container.FindName("TextContentPanel") as StackPanel;
            var errorIndicator = container.FindName("ErrorIndicator") as UIElement;
            var gradientOverlay = container.FindName("GradientOverlay") as Rectangle;
            var effectsBlurPanel = container.FindName("EffectsBlurPanel") as UIElement;
            var imageContainer = container.FindName("ImageContainer") as Border;

            if (loadingStateGrid != null) loadingStateGrid.Visibility = Visibility.Visible;
            if (textContentPanel != null) textContentPanel.Opacity = 0;
            if (gradientOverlay != null) { gradientOverlay.Visibility = Visibility.Collapsed; }
            if (effectsBlurPanel != null) effectsBlurPanel.Visibility = Visibility.Collapsed;
            if (errorIndicator != null) errorIndicator.Visibility = Visibility.Collapsed;
            if (imageContainer != null) imageContainer.Opacity = 0;

            var bitmapImage = new BitmapImage
            {
                UriSource = new Uri(item.Url),
                DecodePixelWidth = 460
            };

            var imageBrush = new ImageBrush { ImageSource = bitmapImage, Stretch = Stretch.UniformToFill };

            bitmapImage.ImageOpened += (s, e) =>
            {
                if (loadingStateGrid != null) loadingStateGrid.Visibility = Visibility.Collapsed;
                if (gradientOverlay != null) { gradientOverlay.Visibility = Visibility.Visible; }
                if (effectsBlurPanel != null) effectsBlurPanel.Visibility = Visibility.Visible;

                var sb = new Storyboard();

                if (imageContainer != null)
                {
                    var imageFadeIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(250) };
                    Storyboard.SetTarget(imageFadeIn, imageContainer);
                    Storyboard.SetTargetProperty(imageFadeIn, "Opacity");
                    sb.Children.Add(imageFadeIn);
                }

                if (textContentPanel != null)
                {
                    var textFadeIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(250) };
                    Storyboard.SetTarget(textFadeIn, textContentPanel);
                    Storyboard.SetTargetProperty(textFadeIn, "Opacity");
                    sb.Children.Add(textFadeIn);
                }

                sb.Begin();
            };

            bitmapImage.ImageFailed += (s, e) =>
            {
                if (loadingStateGrid != null) loadingStateGrid.Visibility = Visibility.Collapsed;
                if (errorIndicator != null) errorIndicator.Visibility = Visibility.Visible;

                if (textContentPanel != null)
                {
                    var textFadeIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(250) };
                    var sb = new Storyboard();
                    Storyboard.SetTarget(textFadeIn, textContentPanel);
                    Storyboard.SetTargetProperty(textFadeIn, "Opacity");
                    sb.Children.Add(textFadeIn);
                    sb.Begin();
                }
            };

            if (imageContainer != null) imageContainer.Background = imageBrush;

            container.Tag = imageBrush;
        }
        private void GamesItemsRepeater_ElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
        {
            var container = args.Element as Grid;
            if (container == null) return;
            var imageBrush = container.Tag as ImageBrush;
            if (imageBrush?.ImageSource is BitmapImage bitmapImage)
            {
                bitmapImage.UriSource = null;
            }

            if (container.FindName("ImageContainer") is Border imageContainer)
            {
                imageContainer.Background = null;
            }

            container.Tag = null;
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