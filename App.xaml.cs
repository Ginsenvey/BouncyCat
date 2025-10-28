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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

            // 捕获 WinUI 未处理异常，记录到本地文件，避免程序直接崩溃无痕
            this.UnhandledException += App_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
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

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
            // 如果想让程序继续运行可以设置为 true，但调试时建议为 false 以触发调试器
            e.Handled = false;
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                LogException(ex);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception);
            e.SetObserved();
        }

        /// <summary>
        /// 此函数记录非打包时的隐藏崩溃记录。
        /// </summary>
        /// <param name="ex"></param>
        private void LogException(Exception ex)
        {
            try
            {
                var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BouncyCat");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.WriteAllText(path, ex.ToString());
            }
            catch { /* 最后手段：避免二次异常 */ }
        }
    }
}
