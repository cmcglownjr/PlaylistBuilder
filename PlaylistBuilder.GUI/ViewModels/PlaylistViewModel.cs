using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PlaylistBuilder.GUI.Models;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        public ObservableCollection<PlaylistTrack> PlaylistTracks { get; }

        public PlaylistViewModel()
        {
            PlaylistTracks = new ObservableCollection<PlaylistTrack>(GenerateTracks());
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