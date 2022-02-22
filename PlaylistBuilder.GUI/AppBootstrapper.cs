using Splat;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.ViewModels;

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
            Locator.CurrentMutable.RegisterConstant(new PreferenceViewModel(), typeof(PreferenceViewModel));
            Locator.CurrentMutable.RegisterConstant(new PlaybackViewModel(), typeof(PlaybackViewModel));
            Locator.CurrentMutable.RegisterConstant(new PlaylistViewModel(), typeof(PlaylistViewModel));
            Locator.CurrentMutable.RegisterConstant(new TrackInfoViewModel(), typeof(TrackInfoViewModel));
            Locator.CurrentMutable.RegisterConstant(new NavigatorViewModel(), typeof(NavigatorViewModel));
            Locator.CurrentMutable.RegisterConstant(new MainWindowViewModel(), typeof(MainWindowViewModel));
        }
    }
}