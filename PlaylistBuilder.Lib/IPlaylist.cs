using ATL;

namespace PlaylistBuilder.Lib;

public interface IPlaylist
{
    public bool Relative { set; }
    public List<Track> ReadList { get; }
    public void LoadPlaylist(string path);
    public void SavePlaylist(string path);
    public void AddTrack(Track track);
    public void RemoveTrack(Track track);
    public void ClearPlaylist();
    public void ShufflePlaylist();
    public void RemoveDuplicates();
    public void RemoveUnavailable();
}