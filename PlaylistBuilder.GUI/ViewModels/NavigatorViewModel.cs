using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using ATL;
using ATL.AudioData;
using ATL.Playlist;
using Avalonia.Controls;
using LibVLCSharp.Shared;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.Lib;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels;

public class NavigatorViewModel : ViewModelBase
{
    private readonly string _musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
    private string _currentDirectory = "";
    private readonly string _rootDirectory =
        Directory.GetDirectoryRoot(Environment.SpecialFolder.Personal.ToString());
    private int _selectedDirectoryIndex;
    private readonly List<string> _mediaExtensions = new();
    private readonly IconModel? _mediaIconModel;
    private List<MediaItemModel> _itemList = new();
    private Stack<string> _undoStack = new();
    private Stack<string> _redoStack = new();
    internal readonly List<string> PlaylistExtensions = new();
    public List<MediaItemModel> ItemList
    {
        get => _itemList;
        private set => this.RaiseAndSetIfChanged(ref _itemList, value);
    }
    public string CurrentDirectory
    {
        get => _currentDirectory;
        set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
    }

    public int SelectedDirectoryIndex
    {
        get => _selectedDirectoryIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedDirectoryIndex, value);
    }
    public ReactiveCommand<Unit, Unit> HomeBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> ParentBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> UndoBtnPressed { get; }
    public ReactiveCommand<Unit, Unit> RedoBtnPressed { get; }

    public NavigatorViewModel()
    {
        _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel))!;
        FindExtensions();
        ItemList = new List<MediaItemModel>(PopulateTree(_musicDirectory));
        HomeBtnPressed = ReactiveCommand.Create(HomeDirectory);
        ParentBtnPressed = ReactiveCommand.Create(ParentDirectory);
        UndoBtnPressed = ReactiveCommand.Create(UndoNavigation);
        RedoBtnPressed = ReactiveCommand.Create(RedoNavigation);
    }
    private void FindExtensions()
    {
        foreach (Format f in PlaylistIOFactory.GetInstance().getFormats())
        {
            if (f.Readable)
            {
                foreach (string extension in f)
                {
                    PlaylistExtensions.Add(extension.Trim('.'));
                }
            }
        }
        foreach (Format f in AudioDataIOFactory.GetInstance().getFormats())
        {
            if (f.Readable)
            {
                foreach (string extension in f)
                {
                    _mediaExtensions.Add(extension);
                }
            }
        }
    }
    private List<MediaItemModel> PopulateTree(string directory)
    {
        ItemList.Clear();
        List<MediaItemModel> itemList = new List<MediaItemModel>();
        DirectoryInfo info = new DirectoryInfo(directory);
        foreach (DirectoryInfo dir in info.GetDirectories().OrderBy(dir => dir.Name))
        {
            if ((dir.Attributes & FileAttributes.Hidden) == 0)
            {
                Image mediaImage = new Image
                {
                    Source = _mediaIconModel?.FolderImage
                };
                itemList.Add(new MediaItemModel(dir, MediaItemType.Directory, mediaImage));
            }
        }

        foreach (FileInfo file in info.GetFiles().OrderBy(file => file.Name))
        {
            if (PlaylistExtensions.Any(file.Extension.Contains))
            {
                Image mediaImage = new Image
                {
                    Source = _mediaIconModel?.PlaylistImage
                };
                itemList.Add(new MediaItemModel(file, MediaItemType.Playlist, mediaImage));
            }
            else if (_mediaExtensions.Any(file.Extension.Contains))
            {
                Image mediaImage = new Image
                {
                    Source = _mediaIconModel?.MusicImage
                };
                itemList.Add(new MediaItemModel(file, MediaItemType.Media, mediaImage));
            }
        }
        CurrentDirectory = directory;
        return itemList;
    }
    private void HomeDirectory()
    {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        _undoStack.Push(CurrentDirectory);
        _redoStack.Clear();
        ItemList = new List<MediaItemModel>(PopulateTree(homeDirectory));
    }
    private void ParentDirectory()
    {
        if (CurrentDirectory != _rootDirectory)
        {
            _undoStack.Push(CurrentDirectory);
            _redoStack.Clear();
            ItemList = new List<MediaItemModel>(PopulateTree(Directory.GetParent(CurrentDirectory).ToString()));
        }
        else
        {
            Log.Warning("You are at the root level!");
        }
    }
    private void TrackNavigation(bool initialDirectory)
    {
        if (!initialDirectory)
        {
            _undoStack.Push(_currentDirectory);
        }
    }
    private void UndoNavigation()
    {
        if (_undoStack.Count > 0)
        {
            _redoStack.Push(CurrentDirectory);
            CurrentDirectory = _undoStack.Peek();
            _undoStack.Pop();
            ItemList = new List<MediaItemModel>(PopulateTree(CurrentDirectory));
        }
    }
    private void RedoNavigation()
    {
        if (_redoStack.Count > 0)
        {
            _undoStack.Push(CurrentDirectory);
            CurrentDirectory = _redoStack.Peek();
            _redoStack.Pop();
            ItemList = new List<MediaItemModel>(PopulateTree(CurrentDirectory));
        }
    }
    public void DblTappedItem()
    {
        PlaylistViewModel playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        PlaybackViewModel playbackViewModel = (PlaybackViewModel)Locator.Current.GetService(typeof(PlaybackViewModel))!;
        MediaItemModel selectedItem = _itemList[_selectedDirectoryIndex];
        switch (selectedItem.FileType)
        {
            case MediaItemType.Directory:
            {
                TrackNavigation(false);
                _redoStack.Clear();
                CurrentDirectory = selectedItem.FullPath;
                ItemList = new List<MediaItemModel>(PopulateTree(CurrentDirectory));
                break;
            }
            case MediaItemType.Media:
            {
                playlistViewModel.PlaylistTracks.Add(new PlaylistTrack(new Track(selectedItem.FullPath)));
                playbackViewModel.PlaylistMedia.Add(new(new Media(playbackViewModel.LibVlc, 
                    new Uri(selectedItem.FullPath))));
                playlistViewModel.UpdatePlaylistTotals();
                Log.Information("Adding {Arg0} tracks to the playlist", 
                    playlistViewModel.PlaylistTracks.Count);
                break;
            }
            case MediaItemType.Playlist:
            {
                FileInfo file = new FileInfo(selectedItem.FullPath);
                try
                {
                    IPlaylist playlist = playlistViewModel.PlaylistHandler(file);
                    playlistViewModel.ImportPlaylist(playlist);
                    Log.Information("Adding an media item to the current playlist");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.Information("Imported playlist is not supported");
                }
                break;
            }
        }
    }
}