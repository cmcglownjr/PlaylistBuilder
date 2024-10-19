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
    private bool _absoluteRadio;
    private bool _relativeRadio;
    private bool _m3uRadio;
    private bool _plsRadio;
    private bool _xspfRadio;

    public bool AbsoluteRadio
    {
        get => _absoluteRadio;
        set => this.RaiseAndSetIfChanged(ref _absoluteRadio, value);
    }

    public bool RelativeRadio
    {
        get => _relativeRadio;
        set => this.RaiseAndSetIfChanged(ref _relativeRadio, value);
    }

    public bool M3URadio
    {
        get => _m3uRadio;
        set => this.RaiseAndSetIfChanged(ref _m3uRadio, value);
    }

    public bool PlsRadio
    {
        get => _plsRadio;
        set => this.RaiseAndSetIfChanged(ref _plsRadio, value);
    }

    public bool XspfRadio
    {
        get => _xspfRadio;
        set => this.RaiseAndSetIfChanged(ref _xspfRadio, value);
    }
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
        RelativeRadio = true;
        M3URadio = true;
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
        Log.Information("Playlist Extension set to {Arg0}", PlaylistExtension);
    }
    private void OnClosePressed(Window window)
    {
        try
        {
            window.Close();
        }
        catch (Exception e)
        {
            //TODO: Fix this error
            Log.Error("Need to fix window close null exception.");
            throw;
        }
    }
}