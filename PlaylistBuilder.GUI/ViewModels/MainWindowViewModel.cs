using System.Reactive;
using PlaylistBuilder.GUI.Views;
using ReactiveUI;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ReactiveCommand<Unit, Unit> PreferenceBtnPressed { get; }

        public MainWindowViewModel()
        {
            PreferenceBtnPressed = ReactiveCommand.Create(OpenPreferenceWindow);
        }

        private void OpenPreferenceWindow()
        {
            var preferenceWindow = new PreferenceWindow();
            preferenceWindow.Show();
        }
    }
}