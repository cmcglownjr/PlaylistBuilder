using System.Reactive;
using Avalonia.Controls;
using PlaylistBuilder.GUI.Models;
using static PlaylistBuilder.GUI.Models.DialogModels;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using ReactiveUI;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OpenBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> SaveBtnPressed { get; }
        public ReactiveCommand<Window, Unit> QuitBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> PreferenceBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> ShuffleBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> RemoveDuplicateBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> RemoveUnavailableBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> AboutPlaylistBuilderBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> AboutAvaloniaBtnPressed { get; }

        public MainWindowViewModel()
        {
            OpenBtnPressed = ReactiveCommand.CreateFromTask(OpenFile);
            SaveBtnPressed = ReactiveCommand.CreateFromTask(SaveFile);
            QuitBtnPressed = ReactiveCommand.Create<Window>(QuitProgram);
            PreferenceBtnPressed = ReactiveCommand.Create(OpenPreferenceWindow);
            ShuffleBtnPressed = ReactiveCommand.Create(ShufflePlaylist);
            RemoveDuplicateBtnPressed = ReactiveCommand.Create(RemoveDuplicates);
            RemoveUnavailableBtnPressed = ReactiveCommand.Create(RemoveUnavailable);
            AboutPlaylistBuilderBtnPressed = ReactiveCommand.Create(AboutPlaylistBuilder);
            AboutAvaloniaBtnPressed = ReactiveCommand.Create(AboutAvalonia);
        }

        private void QuitProgram(Window window)
        {
            window.Close();
        }
        private void OpenPreferenceWindow()
        {
            var preferenceWindow = new PreferenceWindow();
            preferenceWindow.Show();
        }

        private void ShufflePlaylist()
        {
            PlaylistViewModel playlistViewModel = 
                (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
            PlaybackViewModel playbackViewModel = 
                (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
            IPlaylist playlist = new PlaylistM3U();
            foreach (PlaylistTrack track in playlistViewModel.PlaylistTracks)
            {
                playlist.AddTrack(track.Track);
            }
            playbackViewModel.PlaylistMedia.Clear();
            playlistViewModel.PlaylistTracks.Clear();
            playlist.ShufflePlaylist();
            playlistViewModel.ImportPlaylist(playlist);
        }

        private void RemoveDuplicates()
        {
            PlaylistViewModel playlistViewModel = 
                (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
            PlaybackViewModel playbackViewModel = 
                (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
            IPlaylist playlist = new PlaylistM3U();
            foreach (PlaylistTrack track in playlistViewModel.PlaylistTracks)
            {
                playlist.AddTrack(track.Track);
            }
            playbackViewModel.PlaylistMedia.Clear();
            playlistViewModel.PlaylistTracks.Clear();
            playlist.RemoveDuplicates();
            playlistViewModel.ImportPlaylist(playlist);
        }

        private void RemoveUnavailable()
        {
            PlaylistViewModel playlistViewModel = 
                (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
            PlaybackViewModel playbackViewModel = 
                (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
            IPlaylist playlist = new PlaylistM3U();
            foreach (PlaylistTrack track in playlistViewModel.PlaylistTracks)
            {
                playlist.AddTrack(track.Track);
            }
            playbackViewModel.PlaylistMedia.Clear();
            playlistViewModel.PlaylistTracks.Clear();
            playlist.RemoveUnavailable();
            playlistViewModel.ImportPlaylist(playlist);
        }

        private void AboutPlaylistBuilder()
        {
            var window = new AboutPlaylistBuilderView();
            window.Show();
        }

        private void AboutAvalonia()
        {
            var window = new AboutAvaloniaView();
            window.Show();
        }
    }
}