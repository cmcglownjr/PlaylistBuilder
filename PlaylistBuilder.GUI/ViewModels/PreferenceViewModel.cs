using System;
using System.Reactive;
using Avalonia.Controls;
using PlaylistBuilder.GUI.Models;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels;

public class PreferenceViewModel : ViewModelBase
{
    internal bool PlaylistAbsolute;
    internal string PlaylistExtension = "m3u";
    public ReactiveCommand<Unit, Unit> AbsoluteFilePath { get; }
    public ReactiveCommand<Unit, Unit> RelativeFilePath { get; }
    public ReactiveCommand<Unit, Unit> M3UExtension { get; }
    public ReactiveCommand<Unit, Unit> PlsExtension { get; }
    public ReactiveCommand<Unit, Unit> XspfExtension { get; }
    public ReactiveCommand<Window, Unit> CloseButton { get; }

    public PreferenceViewModel()
    {
        AbsoluteFilePath = ReactiveCommand.Create(() => SetPlaylistFilePaths(true));
        RelativeFilePath = ReactiveCommand.Create(() => SetPlaylistFilePaths(false));
        M3UExtension = ReactiveCommand.Create(() => SetPlaylistExtension(Models.PlaylistExtension.M3U));
        PlsExtension = ReactiveCommand.Create(() => SetPlaylistExtension(Models.PlaylistExtension.PLS));
        XspfExtension = ReactiveCommand.Create(() => SetPlaylistExtension(Models.PlaylistExtension.XSPF));
        CloseButton = ReactiveCommand.Create<Window>(OnClosePressed);
    }
    private void SetPlaylistFilePaths(bool setAbsolute)
    {
        PlaylistAbsolute = setAbsolute;
        Log.Information("Playlist absolute paths set to {Arg0}", setAbsolute);
    }
    private void SetPlaylistExtension(PlaylistExtension extension)
    {
        switch (extension)
        {
            case Models.PlaylistExtension.M3U:
                PlaylistExtension = "m3u";
                break;
            case Models.PlaylistExtension.PLS:
                PlaylistExtension = "pls";
                break;
            case Models.PlaylistExtension.XSPF:
                PlaylistExtension = "xspf";
                break;
        }
    }
    private void OnClosePressed(Window window)
    {
        window.Close();
    }
}