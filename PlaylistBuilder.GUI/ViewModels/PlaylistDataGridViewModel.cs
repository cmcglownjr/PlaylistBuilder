using System.Collections.ObjectModel;
using ATL;
using PlaylistBuilder.GUI.Models;

namespace PlaylistBuilder.GUI.ViewModels;

public class PlaylistDataGridViewModel
{
    private ObservableCollection<PlaylistTrack> _playlistTracks;
    public ObservableCollection<PlaylistTrack> PlaylistTracks { get; }

    public PlaylistDataGridViewModel()
    {
        PlaylistTracks = new ObservableCollection<PlaylistTrack>();
        var path = @"/home/eezyville/Programming/Rider/PlaylistBuilder/PlaylistBuilder.GUI/01 - Purple Haze.mp3";
        Track newTrack = new Track(path);
        var test = newTrack.AudioFormat.ShortName;
        PlaylistTracks.Add(new PlaylistTrack(newTrack));
    }
}