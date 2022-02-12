using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.Views;
using ReactiveUI;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        private ObservableCollection<PlaylistTabModel> _playlistTabModels = new();
        private int _playlistItem;

        public ObservableCollection<PlaylistTabModel> PlaylistTabModels
        {
            get => _playlistTabModels;
            set => this.RaiseAndSetIfChanged(ref _playlistTabModels, value);
        }

        public int PlaylistItem
        {
            get => _playlistItem;
            set => this.RaiseAndSetIfChanged(ref _playlistItem, value);
        }
        public ReactiveCommand<Unit, Unit> NewBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> CloseBtnPressed { get; }
        public PlaylistViewModel()
        {
            NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
            CloseBtnPressed = ReactiveCommand.Create(ClosePlaylist);
        }

        private void NewPlaylist()
        {
            PlaylistTabModels.Add(new PlaylistTabModel($"Playlist {PlaylistTabModels.Count + 1}", new PlaylistDataGrid()));
        }

        private void ClosePlaylist()
        {
            PlaylistTabModels.RemoveAt(PlaylistItem);
        }
    }
}