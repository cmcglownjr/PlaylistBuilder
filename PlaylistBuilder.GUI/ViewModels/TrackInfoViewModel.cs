using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reactive;
using ATL;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PlaylistBuilder.GUI.Models;
using ReactiveUI;
using Serilog;
using Splat;
using static PlaylistBuilder.GUI.Models.UpdateItem;

namespace PlaylistBuilder.GUI.ViewModels;

public class TrackInfoViewModel : ViewModelBase
{
    private readonly Image _trackImage = new();
    private readonly IconModel? _mediaIconModel;
    private int _playlistIndex;
    private string _trackAlbum = "No Media";
    private string _trackArtist = "No Media";
    private string _trackTitle = "No Media";
    private string _length = "";
    private string _bitRate = "";
    private string _sampleRate = "";
    private string _fileSize = "";
    private string _fileType = "";
    private string _fileName = "";
    private string _setTitle = "";
    private string _setArtist = "";
    private string _setAlbum = "";
    private string _setAlbumArtist = "";
    private string? _setTrack = "";
    private string? _setDisc = "";
    private string? _setYear = "";

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

    public string Length
    {
        get => _length; 
        set => this.RaiseAndSetIfChanged(ref _length, value);
    }
    public string BitRate
    {
        get => _bitRate; 
        set => this.RaiseAndSetIfChanged(ref _bitRate, value);
    }
    public string SampleRate
    {
        get => _sampleRate; 
        set => this.RaiseAndSetIfChanged(ref _sampleRate, value);
    }
    public string FileSize
    {
        get => _fileSize; 
        set => this.RaiseAndSetIfChanged(ref _fileSize, value);
    }
    public string FileType
    {
        get => _fileType; 
        set => this.RaiseAndSetIfChanged(ref _fileType, value);
    }
    public string FileName
    {
        get => _fileName; 
        set => this.RaiseAndSetIfChanged(ref _fileName, value);
    }
    public string SetTitle
    {
        get => _setTitle; 
        set => this.RaiseAndSetIfChanged(ref _setTitle, value);
    }
    public string SetArtist
    {
        get => _setArtist; 
        set => this.RaiseAndSetIfChanged(ref _setArtist, value);
    }
    public string SetAlbum
    {
        get => _setAlbum; 
        set => this.RaiseAndSetIfChanged(ref _setAlbum, value);
    }
    public string SetAlbumArtist
    {
        get => _setAlbumArtist; 
        set => this.RaiseAndSetIfChanged(ref _setAlbumArtist, value);
    }
    public string? SetTrack
    {
        get => _setTrack; 
        set => this.RaiseAndSetIfChanged(ref _setTrack, value);
    }
    public string? SetDisc
    {
        get => _setDisc; 
        set => this.RaiseAndSetIfChanged(ref _setDisc, value);
    }
    public string? SetYear
    {
        get => _setYear; 
        set => this.RaiseAndSetIfChanged(ref _setYear, value);
    }
    public ReactiveCommand<Unit, Unit>? PreviousBtnPressed { get; }
    public ReactiveCommand<Unit, Unit>? NextBtnPressed { get; }
    public ReactiveCommand<Unit, Unit>? SaveBtnPressed { get; }
    public ReactiveCommand<Window, Unit>? DiscardBtnPressed { get; }

    public TrackInfoViewModel(){ }
    

    public TrackInfoViewModel(PlaylistTrack track)
    {
        _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel))!;
        var playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        _playlistIndex = playlistViewModel.SelectedPlaylistIndex;
        PreviousBtnPressed = ReactiveCommand.Create(()=>CycleItems(false));
        NextBtnPressed = ReactiveCommand.Create(()=>CycleItems(true));
        SaveBtnPressed = ReactiveCommand.Create(SaveItem);
        DiscardBtnPressed = ReactiveCommand.Create<Window>(DiscardBtn);
        GetTrackInfo(track);
    }

    private Bitmap? AlbumImage(PlaylistTrack track)
    {
        if (track.Track.EmbeddedPictures.Count > 0)
        {
            IList<PictureInfo> embeddedPictures = track.Track.EmbeddedPictures;
            var image = System.Drawing.Image.FromStream(new MemoryStream(embeddedPictures[0].PictureData));
            using var memory = new MemoryStream();
            image.Save(memory, ImageFormat.Jpeg);
            memory.Position = 0;
            return new Bitmap(memory);
        }
        return _mediaIconModel?.CDImage;
    }

    private void GetTrackInfo(PlaylistTrack track)
    {
        var trackInfo = new FileInfo(track.Track.Path);
        TrackImage.Source = AlbumImage(track);
        TrackAlbum = track.Album;
        TrackArtist = track.Artist;
        TrackTitle = track.Title;
        Length = $"{track.Duration}";
        BitRate = $"{track.Track.Bitrate} kbps";
        SampleRate = $"{track.Track.SampleRate} Hz";
        FileSize = $"{(trackInfo.Length/1024f/1024f):N} MB";
        FileType = trackInfo.Extension;
        FileName = track.FileName;
        SetTitle = track.Title;
        SetArtist = track.Artist;
        SetAlbum = track.Album;
        SetAlbumArtist = track.Track.AlbumArtist;
        SetTrack = track.Track.TrackNumber.ToString();
        SetDisc = track.Track.DiscNumber.ToString();
        SetYear = track.Track.Year.ToString();
        Log.Information("Gathering information for track {Arg0}", track.Title);
    }

    private void CycleItems(bool forward)
    {
        var playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        try
        {
            if (forward)
            {
                GetTrackInfo(playlistViewModel.PlaylistTracks[_playlistIndex + 1]);
                _playlistIndex += 1;
            }
            else
            {
                GetTrackInfo(playlistViewModel.PlaylistTracks[_playlistIndex - 1]);
                _playlistIndex -= 1;
            }
        }
        catch (ArgumentOutOfRangeException e)
        {
            Log.Warning(e,"You are moving items outside of the list!");
        }
    }

    private void SaveItem()
    {
        var playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        var editTrack = playlistViewModel.PlaylistTracks[_playlistIndex].Track;
        editTrack.Title = SetTitle;
        editTrack.Artist = SetArtist;
        editTrack.Album = SetAlbum;
        editTrack.AlbumArtist = SetAlbumArtist;
        if (int.TryParse(SetTrack, out var trackNumber))
        {
            editTrack.TrackNumber = trackNumber;
        }
        if (int.TryParse(SetDisc, out var discNumber))
        {
            editTrack.DiscNumber = discNumber;
        }
        if (int.TryParse(SetYear, out var year))
        {
            editTrack.Year = year;
        }

        editTrack.Save();
        UpdateListItem(playlistViewModel.PlaylistTracks, _playlistIndex, new PlaylistTrack(editTrack));
        Log.Information("Saving track info for {Arg0}", editTrack.Title);
    }

    private static void DiscardBtn(Window window)
    {
        window.Close();
    }
}