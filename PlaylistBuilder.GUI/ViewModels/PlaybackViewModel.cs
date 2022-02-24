using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reactive;
using ATL;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using LibVLCSharp.Shared;
using PlaylistBuilder.GUI.Models;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels;

public class PlaybackViewModel:ViewModelBase
{
    private Image _trackImage = new();
    private readonly IconModel? _mediaIconModel;
    private string _trackAlbum = "No Media";
    private string _trackArtist = "No Media";
    private string _trackTitle = "No Media";
    private string _playlistDetails = "0 tracks - [00:00:00]";
    internal LibVLC LibVlc;
    public List<MediaPlayer> PlaylistMedia = new();
    public Image TrackImage => _trackImage;
    public string TrackAlbum
    {
        get => _trackAlbum; 
        set => this.RaiseAndSetIfChanged(ref _trackAlbum, value);
    }

    public string TrackArtist
    {
        get => _trackArtist; 
        set => this.RaiseAndSetIfChanged(ref _trackArtist, value);
    }

    public string TrackTitle
    {
        get => _trackTitle; 
        set => this.RaiseAndSetIfChanged(ref _trackTitle, value);
    }
    public string PlaylistDetails
    {
        get => _playlistDetails;
        set => this.RaiseAndSetIfChanged(ref _playlistDetails, value);
    }
    public ReactiveCommand<Unit, Unit> PlayBtn { get; }
    public ReactiveCommand<Unit, Unit> PauseBtn { get; }
    public ReactiveCommand<Unit, Unit> StopBtn { get; }
    public ReactiveCommand<Unit, Unit> NextBtn { get; }
    public ReactiveCommand<Unit, Unit> PreviousBtn { get; }

    public PlaybackViewModel()
    {
        Core.Initialize();
        LibVlc = new(false);
        _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel))!;
        TrackImage.Source = _mediaIconModel.CDImage;
        PlayBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Play));
        PauseBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Pause));
        StopBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Stop));
        NextBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Next));
        PreviousBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Previous));
    }
    internal void MediaPlayback(PlaybackControl control)
    {
        PlaylistViewModel playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        switch (control)
        {
            case PlaybackControl.Play:
            {
                foreach (MediaPlayer media in PlaylistMedia)
                {
                    media.Stop();
                }
                PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Play();
                NowPlaying(true);
                break;
            }
            case PlaybackControl.Pause:
            {
                PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Pause();
                break;
            }
            case PlaybackControl.Stop:
            {
                foreach (MediaPlayer media in PlaylistMedia)
                {
                    media.Stop();
                }
                NowPlaying(false);
                break;
            }
            case PlaybackControl.Next:
            {
                try
                {
                    PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Stop();
                    playlistViewModel.SelectedPlaylistIndex += 1;
                    PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Play();
                    NowPlaying(true);
                }
                catch (Exception e)
                {
                    Log.Warning(e, "End of playlist");
                }
                break;
            }
            case PlaybackControl.Previous:
            {
                try
                {
                    PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Stop();
                    playlistViewModel.SelectedPlaylistIndex -= 1;
                    PlaylistMedia[playlistViewModel.SelectedPlaylistIndex].Play();
                    NowPlaying(true);
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Beginning of playlist");
                }
                break;
            }
        }
    }
    internal void NowPlaying(bool show)
    {
        PlaylistViewModel playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        if (show)
        {
            Track track = playlistViewModel.PlaylistTracks[playlistViewModel.SelectedPlaylistIndex].Track;
            if (track.EmbeddedPictures.Count > 0)
            {
                IList<PictureInfo> embeddedPictures = track.EmbeddedPictures;
                System.Drawing.Image image =
                    System.Drawing.Image.FromStream(new MemoryStream(embeddedPictures[0].PictureData));
                using (MemoryStream memory = new MemoryStream())
                {
                    image.Save(memory, ImageFormat.Jpeg);
                    memory.Position = 0;
                    TrackImage.Source = new Bitmap(memory);
                }
            }
            else
            {
                TrackImage.Source = _mediaIconModel?.CDImage;
            }
            TrackAlbum = track.Album;
            TrackArtist = track.Artist;
            TrackTitle = track.Title;
            Log.Information("Now playing {Arg0}", track.Title);
        }
        else
        {
            TrackImage.Source = _mediaIconModel?.CDImage;
            TrackAlbum = "No Media";
            TrackArtist = "No Media";
            TrackTitle = "No Media";
        }
    }
}