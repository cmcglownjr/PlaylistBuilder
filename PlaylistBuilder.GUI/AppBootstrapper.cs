using Splat;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.ViewModels;
using PlaylistBuilder.GUI.Views;

namespace PlaylistBuilder.GUI
{
    public class AppBootstrapper
    {
        public AppBootstrapper()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            Locator.CurrentMutable.RegisterConstant(new IconModel(), typeof(IconModel));
            Locator.CurrentMutable.RegisterConstant(new DirectoryViewModel(), typeof(DirectoryViewModel));
            Locator.CurrentMutable.RegisterConstant(new PlaylistView(), typeof(PlaylistView));
            Locator.CurrentMutable.RegisterConstant(new PlaybackView(), typeof(PlaybackView));
            Locator.CurrentMutable.RegisterConstant(new MainWindowViewModel(), typeof(MainWindowViewModel));
        }
    }
}