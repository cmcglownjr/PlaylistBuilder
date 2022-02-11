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
        private List<PlaylistTabModel> _playlistTabModels = new();
        public ObservableCollection<PlaylistTrack> PlaylistTracks { get; }

        public List<PlaylistTabModel> PlaylistTabModels
        {
            get => _playlistTabModels;
            set => this.RaiseAndSetIfChanged(ref _playlistTabModels, value);
        }
        public ReactiveCommand<Unit, Unit> NewBtnPressed { get; }
        public PlaylistViewModel()
        {
            PlaylistTracks = new ObservableCollection<PlaylistTrack>(GenerateTracks());
            NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
            PlaylistTabModels.Add(new PlaylistTabModel("Playlist 1", new PlaylistDataGrid()));
            PlaylistTabModels.Add(new PlaylistTabModel("Playlist 2", new PlaylistDataGrid()));
        }

        private void NewPlaylist()
        {
            //TODO: Create new tabbed playlist
        }

        private IEnumerable<PlaylistTrack> GenerateTracks()
        {
            var defaultTracks = new List<PlaylistTrack>()
            {
                new PlaylistTrack()
                {
                    Track = 5,
                    Title = "Breath of the Wild: World Medley -Final Battle-",
                    Artist = "Tokyo Philharmonic Orchestra",
                    Album = "The Legend of Zelda Concert 2018",
                    Duration = new TimeSpan(0, 12, 16),
                    FileName = "05 - Breath of the Wild- World Medley -Final Battle-.ogg"
                },
                new PlaylistTrack()
                {
                    Track = 7,
                    Title = "Breath of the Wild- Champion Medley",
                    Artist = "Tokyo Philharmonic Orchestra",
                    Album = "The Legend of Zelda Concert 2018",
                    Duration = new TimeSpan(0, 11, 12),
                    FileName = "07 - Breath of the Wild- Champion Medley.ogg"
                },
                new PlaylistTrack()
                {
                    Track = 49,
                    Title = "Molgera (The Legend of Zelda: The Wind Waker HD)",
                    Artist = "Nintendo",
                    Album = "The 30th Anniversary The Legend of Zelda Game Music Collection",
                    Duration = new TimeSpan(0, 2, 28),
                    FileName = "49 Nintendo - Molgera (The Legend of Zelda- The Wind Waker HD).flac"
                },
                new PlaylistTrack()
                {
                    Track = 1,
                    Title = "Purple Haze",
                    Artist = "Jimi Hendrix",
                    Album = "Greatest Hits",
                    Duration = new TimeSpan(0, 2, 51),
                    FileName = "01 - Purple Haze.mp3"
                }
            };
            return defaultTracks;
        }
    }
}