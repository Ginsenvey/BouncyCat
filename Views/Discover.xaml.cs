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
        }

        private void st_Checked(object sender, RoutedEventArgs e)
        {
            Wrapper.IsPaneOpen = !Wrapper.IsPaneOpen;
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
    }
}
