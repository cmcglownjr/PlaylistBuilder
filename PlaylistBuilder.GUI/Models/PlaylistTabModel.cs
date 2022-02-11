using PlaylistBuilder.GUI.Views;

namespace PlaylistBuilder.GUI.Models;

public class PlaylistTabModel
{
    public string Header { get; }
    public PlaylistDataGrid PlaylistData { get; }

    public PlaylistTabModel(string header, PlaylistDataGrid playlistData)
    {
        Header = header;
        PlaylistData = playlistData;
    }
}