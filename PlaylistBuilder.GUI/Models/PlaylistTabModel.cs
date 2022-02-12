using System.Reactive;
using PlaylistBuilder.GUI.ViewModels;
using PlaylistBuilder.GUI.Views;
using ReactiveUI;
using Splat;

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