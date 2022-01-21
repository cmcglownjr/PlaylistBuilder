using ATL;

namespace PlaylistBuilder.Lib;

public class PlaylistPLS : IPlaylist
{
    private string _header = "[playlist]";
    private List<Track> _playlist = new();
    
    public void LoadPlaylist(string path)
    {
        throw new NotImplementedException();
    }

    public void SavePlaylist(string path)
    {
        throw new NotImplementedException();
    }

    public void AddTrack(Track track)
    {
        throw new NotImplementedException();
    }

    public void RemoveTrack(Track track)
    {
        throw new NotImplementedException();
    }

    public void ClearPlaylist()
    {
        throw new NotImplementedException();
    }

    public void ShufflePlaylist()
    {
        throw new NotImplementedException();
    }

    public void RemoveDuplicates()
    {
        throw new NotImplementedException();
    }

    public void RemoveUnavailable()
    {
        throw new NotImplementedException();
    }
}