using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using PlaylistBuilder.GUI.Views;
using Splat;

namespace PlaylistBuilder.GUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            new AppBootstrapper();
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    // DataContext = new MainWindowViewModel(),
                    DataContext = Locator.Current.GetService(typeof(MainWindowViewModel))
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}