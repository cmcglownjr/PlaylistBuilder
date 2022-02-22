using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using ATL;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using LibVLCSharp.Shared;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels;

public class PlaylistViewModel : ViewModelBase
{
    private readonly MainWindow _mainWindow;
    private MainWindowViewModel _mainWindowViewModel;
    private PlaybackViewModel _playbackViewModel;
    private PreferenceViewModel _preferenceViewModel;
    private int _selectedPlaylistIndex;
    private readonly List<string> _playlistExtensions = new();
    private readonly List<string> _mediaExtensions = new();
    public ObservableCollection<PlaylistTrack> PlaylistTracks { get; set; }
    public int SelectedPlaylistIndex
    {
        get => _selectedPlaylistIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistIndex, value);
    }
    public ReactiveCommand<Unit, Unit> NewBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> OpenBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> SaveBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> SwapUpBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> SwapDownBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> RemoveBtnPressed { get; }

    public PlaylistViewModel()
    {
        _mainWindow = new MainWindow();
        _mainWindowViewModel = (MainWindowViewModel)Locator.Current.GetService(typeof(MainWindowViewModel))!;
        _playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        _preferenceViewModel = (PreferenceViewModel)Locator.Current.GetService(typeof(PreferenceViewModel))!;
        NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
        OpenBtnPressed = ReactiveCommand.CreateFromTask(OpenFile);
        SaveBtnPressed = ReactiveCommand.CreateFromTask(SavePlaylist);
        SwapUpBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(true));
        SwapDownBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(false));
        RemoveBtnPressed = ReactiveCommand.Create(RemoveTrack);
    }
    public void DblTappedPlaylist()
    {
        TrackInfoWindow trackInfo = new TrackInfoWindow();
        trackInfo.Show();
    }
    private void NewPlaylist()
    {
        if (_playbackViewModel.PlaylistMedia.Count > 0)
        {
            _playbackViewModel.MediaPlayback(PlaybackControl.Stop);
        }
        _playbackViewModel.NowPlaying(false);
        PlaylistTracks.Clear();
        _playbackViewModel.PlaylistMedia.Clear();
        UpdatePlaylistTotals();
    }
    private void ImportPlaylist(IPlaylist playlist)
    {
        if (_playbackViewModel.PlaylistMedia.Count > 0)
        {
            _playbackViewModel.MediaPlayback(PlaybackControl.Stop);
        }
        PlaylistTracks.Clear();
        _playbackViewModel.NowPlaying(false);
        foreach (Track track in playlist.ReadList)
        {
            PlaylistTracks.Add(new PlaylistTrack(track));
            _playbackViewModel.PlaylistMedia.Add(new(new Media(_playbackViewModel.LibVlc, new Uri(track.Path))));
        }
        UpdatePlaylistTotals();
    }
    private async Task SavePlaylist()
    {
        SaveFileDialog dialog = new();
        dialog.Filters.Add(new FileDialogFilter{Name = $"Playlists ({_preferenceViewModel.PlaylistExtension})", Extensions = _playlistExtensions});
        dialog.Title = $"Save Playlist as {_preferenceViewModel.PlaylistExtension}";
        dialog.DefaultExtension = _preferenceViewModel.PlaylistExtension;
        string result = await dialog.ShowAsync(_mainWindow);
        if (result != null)
        {
            try
            {
                IPlaylist playlist = PlaylistHandler(new FileInfo(result));
                foreach (PlaylistTrack playlistTrack in PlaylistTracks)
                {
                    playlist.AddTrack(playlistTrack.Track);
                }
                playlist.Relative = !_preferenceViewModel.PlaylistAbsolute;
                playlist.SavePlaylist(result);
                Log.Information("Playlist saved to {Arg0}", result);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Information("Imported playlist is not supported");
            }
        }
    }
    private async Task OpenFile()
    {
        OpenFileDialog dialog = new();
        dialog.Filters.Add(new FileDialogFilter{Name = "Playlists", 
            Extensions = _playlistExtensions});
        dialog.AllowMultiple = false;
        dialog.Title = "Open File";
        string[] result = await dialog.ShowAsync(_mainWindow);
        if (result != null)
        {
            foreach (string filepath in result)
            {
                FileInfo file = new(filepath);
                try
                {
                    IPlaylist playlist = PlaylistHandler(file);
                    ImportPlaylist(playlist);
                    Log.Information("Playlist imported from {Arg0}", result);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.Information("Imported playlist is not supported");
                }
            }
        }
    }
    private IPlaylist PlaylistHandler(FileInfo importPlaylist)
    {
        IPlaylist playlist = importPlaylist.Extension switch
        {
            ".m3u" or ".M3U" => new PlaylistM3U(),
            ".pls" or ".PLS" => new PlaylistPLS(),
            ".xspf" or ".XSPF" => new PlaylistXSPF(),
            _ => throw new ArgumentOutOfRangeException()
        };
        playlist.LoadPlaylist(importPlaylist.FullName);
        return playlist;
    }
    private void UpdatePlaylistTotals()
    {
        int totalTracks = PlaylistTracks.Count;
        int totalTime = 0;
        foreach (PlaylistTrack track in PlaylistTracks)
        {
            totalTime += track.Track.Duration;
        }
        _playbackViewModel.PlaylistDetails = $"{totalTracks} tracks - [{TimeSpan.FromSeconds(totalTime)}]";
    }

    private void MovePlaylistItem(bool moveUp)
    {
        int index = SelectedPlaylistIndex;
        try
        {
            if (moveUp)
            {
                MoveItem.MoveListItem(PlaylistTracks, index, index - 1);
                MoveItem.MoveListItem(_playbackViewModel.PlaylistMedia, index, index - 1);
                SelectedPlaylistIndex = index - 1;
            }
            else
            {
                MoveItem.MoveListItem(PlaylistTracks, index, index + 1);
                MoveItem.MoveListItem(_playbackViewModel.PlaylistMedia, index, index + 1);
                SelectedPlaylistIndex = index + 1;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Log.Warning("You are moving items outside of the list!");
            // Log.Error("{@Arg0}", e);
        }
            
    }

    private void RemoveTrack()
    {
        int trackIndex = SelectedPlaylistIndex;
        PlaylistTrack track = PlaylistTracks[SelectedPlaylistIndex];
        PlaylistTracks.RemoveAt(trackIndex);
        _playbackViewModel.PlaylistMedia.RemoveAt(trackIndex);
        UpdatePlaylistTotals();
        Log.Information("Removing '{Arg0}' from playlist", track.Title);
    }
}