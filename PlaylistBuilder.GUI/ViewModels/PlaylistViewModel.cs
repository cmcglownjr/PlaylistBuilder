using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using ATL;
using LibVLCSharp.Shared;
using PlaylistBuilder.GUI.Models;
using static PlaylistBuilder.GUI.Models.DialogModels;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels;

public class PlaylistViewModel : ViewModelBase
{
    private int _selectedPlaylistIndex;
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
        NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
        OpenBtnPressed = ReactiveCommand.CreateFromTask(OpenFile);
        SaveBtnPressed = ReactiveCommand.CreateFromTask(SaveFile);
        SwapUpBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(true));
        SwapDownBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(false));
        RemoveBtnPressed = ReactiveCommand.Create(RemoveTrack);
        PlaylistTracks = new();
    }
    public void DblTappedPlaylist()
    {
        TrackInfoWindow trackInfo = new TrackInfoWindow(PlaylistTracks[SelectedPlaylistIndex]);
        trackInfo.Show();
    }
    private void NewPlaylist()
    {
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        if (playbackViewModel.PlaylistMedia.Count > 0)
        {
            playbackViewModel.MediaPlayback(PlaybackControl.Stop);
        }
        playbackViewModel.NowPlaying(false);
        PlaylistTracks.Clear();
        playbackViewModel.PlaylistMedia.Clear();
        UpdatePlaylistTotals();
    }
    internal void ImportPlaylist(IPlaylist playlist)
    {
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        if (playbackViewModel.PlaylistMedia.Count > 0)
        {
            playbackViewModel.MediaPlayback(PlaybackControl.Stop);
        }
        PlaylistTracks.Clear();
        playbackViewModel.NowPlaying(false);
        foreach (Track track in playlist.ReadList)
        {
            PlaylistTracks.Add(new PlaylistTrack(track));
            playbackViewModel.PlaylistMedia.Add(new(new Media(playbackViewModel.LibVlc, new Uri(track.Path))));
        }
        UpdatePlaylistTotals();
    }
    internal IPlaylist PlaylistHandler(FileInfo importPlaylist)
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
    internal void UpdatePlaylistTotals()
    {
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        int totalTracks = PlaylistTracks.Count;
        int totalTime = 0;
        foreach (PlaylistTrack track in PlaylistTracks)
        {
            totalTime += track.Track.Duration;
        }
        playbackViewModel.PlaylistDetails = $"{totalTracks} tracks - [{TimeSpan.FromSeconds(totalTime)}]";
    }

    private void MovePlaylistItem(bool moveUp)
    {
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        int index = SelectedPlaylistIndex;
        try
        {
            if (moveUp)
            {
                MoveItem.MoveListItem(PlaylistTracks, index, index - 1);
                MoveItem.MoveListItem(playbackViewModel.PlaylistMedia, index, index - 1);
                SelectedPlaylistIndex = index - 1;
            }
            else
            {
                MoveItem.MoveListItem(PlaylistTracks, index, index + 1);
                MoveItem.MoveListItem(playbackViewModel.PlaylistMedia, index, index + 1);
                SelectedPlaylistIndex = index + 1;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Log.Warning(e,"You are moving items outside of the list!");
        }
    }

    private void RemoveTrack()
    {
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        int trackIndex = SelectedPlaylistIndex;
        PlaylistTrack track = PlaylistTracks[SelectedPlaylistIndex];
        PlaylistTracks.RemoveAt(trackIndex);
        playbackViewModel.PlaylistMedia.RemoveAt(trackIndex);
        UpdatePlaylistTotals();
        Log.Information("Removing '{Arg0}' from playlist", track.Title);
    }
}