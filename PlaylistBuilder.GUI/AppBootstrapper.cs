using Splat;
using PlaylistBuilder.Lib;
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
            Locator.CurrentMutable.RegisterConstant(new MainWindowViewModel(), typeof(MainWindowViewModel));
        }
    }
}