using ATL;
using ATL.Playlist;

namespace PlaylistBuilder.Lib;

public abstract class PlaylistBase : IPlaylist
{
    protected bool _relative;
    protected List<Track> _playlist = new();
    private FileInfo _loadedPlaylist;
    protected FileInfo _savedPlaylist;
    public bool Relative { set => _relative = value; }
    public List<Track> ReadList => _playlist;
    public void LoadPlaylist(string path)
    {
        ClearPlaylist();
        _loadedPlaylist = new FileInfo(path);
        ReadPlaylist(_loadedPlaylist);
    }

    public void SavePlaylist(string path)
    {
        _savedPlaylist = new FileInfo(path);
        CreateFile();
    }

    public void AddTrack(Track track)
    {
        _playlist.Add(track);
    }

    public void RemoveTrack(Track track)
    {
        //TODO: I need to refactor this
        foreach (Track playlistTrack in _playlist)
        {
            if (playlistTrack.Path == track.Path)
            {
                _playlist.Remove(playlistTrack);
                break;
            }
        }
    }

    public void ClearPlaylist()
    {
        _playlist.Clear();
    }

    public void ShufflePlaylist()
    {
        throw new NotImplementedException();
    }

    public void RemoveDuplicates()
    {
        Dictionary<Track, string> trackDict = new();
        foreach (Track track in _playlist)
        {
            trackDict.Add(track, $"{track.TrackNumber}:{track.Title}:{track.Album}:{track.Artist}");
        }

        var uniqueValues = trackDict.GroupBy(pair => pair.Value)
            .Select(group => group.First())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        ClearPlaylist();
        foreach (Track unique in uniqueValues.Keys)
        {
            _playlist.Add(unique);
        }
    }

    public void RemoveUnavailable()
    {
        List<Track> tempPlaylist = new List<Track>();
        foreach (Track track in _playlist)
        {
            if (File.Exists(track.Path))
            {
                tempPlaylist.Add(track);
            }
        }
        _playlist = tempPlaylist;
    }

    protected abstract void CreateFile();
    private void ReadPlaylist(FileInfo playlistPath)
    {
        IPlaylistIO reader = PlaylistIOFactory.GetInstance().GetPlaylistIO(playlistPath.FullName);
        foreach (Track track in reader.Tracks)
        {
            _playlist.Add(track);
        }
    }
}