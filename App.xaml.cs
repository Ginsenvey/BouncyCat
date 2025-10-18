using BouncyCat.Services;
using BouncyCat.ViewModels;
using BouncyCat.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BouncyCat
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }
        public static IServiceProvider Services { get; private set; } = null!;
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //对于未打包的WinUI应用，需要用 Environment.GetFolderPath 来获取标准本地应用程序数据路径.
            //string path = ApplicationData.Current.LocalCacheFolder.Path + "/" + "Data.db";
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = System.IO.Path.Combine(localAppDataPath, "BouncyCat", "Data.db");
            Services = new ServiceCollection()
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<IUpdateService>(provider => new UpdateService(path))
            .AddTransient<MainViewModel>()
            .AddTransient<DiscoverViewModel>()
            .BuildServiceProvider();
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
