using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ATL;
using ATL.AudioData;
using ATL.Playlist;
using LibVLCSharp.Shared;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PlaylistBuilder.GUI.Models;
using static PlaylistBuilder.GUI.Models.MoveItem;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using ReactiveUI;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private readonly string _musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private string _currentDirectory = "";
        private readonly string _rootDirectory =
            Directory.GetDirectoryRoot(Environment.SpecialFolder.Personal.ToString());
        private string _playlistExtension = "m3u";
        private string _trackAlbum = "No Media";
        private string _trackArtist = "No Media";
        private string _trackTitle = "No Media";
        private int _selectedDirectoryIndex;
        private int _selectedPlaylistIndex;
        private bool _playlistAbsolute;
        private readonly List<string> _playlistExtensions = new();
        private readonly List<string> _mediaExtensions = new();
        private readonly IconModel? _mediaIconModel;
        private List<MediaItemModel> _itemList = new();
        private Stack<string> _undoStack = new();
        private Stack<string> _redoStack = new();
        private string _playlistDetails = "0 tracks - [00:00:00]";
        private LibVLC _libVlc;
        private List<MediaPlayer> _playlistMedia = new();
        private Image _trackImage = new();
        public ObservableCollection<PlaylistTrack> PlaylistTracks { get; set; }

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
        public int SelectedPlaylistIndex
        {
            get => _selectedPlaylistIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedPlaylistIndex, value);
        }
        public string PlaylistTotals
        {
            get => _playlistDetails;
            set => this.RaiseAndSetIfChanged(ref _playlistDetails, value);
        }

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
        public ReactiveCommand<Unit, Unit> HomeBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> ParentBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> UndoBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> RedoBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> NewBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> OpenBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> SaveBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> SwapUpBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> SwapDownBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> RemoveBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> PreferenceBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> AbsoluteFilePath { get; }
        public ReactiveCommand<Unit, Unit> RelativeFilePath { get; }
        public ReactiveCommand<Unit, Unit> M3UExtension { get; }
        public ReactiveCommand<Unit, Unit> PlsExtension { get; }
        public ReactiveCommand<Unit, Unit> XspfExtension { get; }
        public ReactiveCommand<Window, Unit> CloseButton { get; }
        public ReactiveCommand<Unit, Unit> PlayBtn { get; }
        public ReactiveCommand<Unit, Unit> PauseBtn { get; }
        public ReactiveCommand<Unit, Unit> StopBtn { get; }
        public ReactiveCommand<Unit, Unit> NextBtn { get; }
        public ReactiveCommand<Unit, Unit> PreviousBtn { get; }

        public MainWindowViewModel()
        {
            Core.Initialize();
            _libVlc = new(true);
            _mainWindow = new MainWindow();
            _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel))!;
            TrackImage.Source = _mediaIconModel.CDImage;
            FindExtensions();
            ItemList = new List<MediaItemModel>(PopulateTree(_musicDirectory));
            HomeBtnPressed = ReactiveCommand.Create(HomeDirectory);
            ParentBtnPressed = ReactiveCommand.Create(ParentDirectory);
            UndoBtnPressed = ReactiveCommand.Create(UndoNavigation);
            RedoBtnPressed = ReactiveCommand.Create(RedoNavigation);
            NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
            OpenBtnPressed = ReactiveCommand.CreateFromTask(OpenFile);
            SaveBtnPressed = ReactiveCommand.CreateFromTask(SavePlaylist);
            SwapUpBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(true));
            SwapDownBtnPressed = ReactiveCommand.Create(() => MovePlaylistItem(false));
            RemoveBtnPressed = ReactiveCommand.Create(RemoveTrack);
            PreferenceBtnPressed = ReactiveCommand.Create(OpenPreferenceWindow);
            AbsoluteFilePath = ReactiveCommand.Create(() => SetPlaylistFilePaths(true));
            RelativeFilePath = ReactiveCommand.Create(() => SetPlaylistFilePaths(false));
            M3UExtension = ReactiveCommand.Create(() => SetPlaylistExtension(PlaylistExtension.M3U));
            PlsExtension = ReactiveCommand.Create(() => SetPlaylistExtension(PlaylistExtension.PLS));
            XspfExtension = ReactiveCommand.Create(() => SetPlaylistExtension(PlaylistExtension.XSPF));
            CloseButton = ReactiveCommand.Create<Window>(OnClosePressed);
            PlayBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Play));
            PauseBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Pause));
            StopBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Stop));
            NextBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Next));
            PreviousBtn = ReactiveCommand.Create(() => MediaPlayback(PlaybackControl.Previous));
            PlaylistTracks = new();
        }

        private void OpenPreferenceWindow()
        {
            var prefenceWindow = new PreferenceWindow();
            prefenceWindow.Show();
        }

        private void SetPlaylistFilePaths(bool setAbsolute)
        {
            _playlistAbsolute = setAbsolute;
            Log.Information("Playlist absolute paths set to {Arg0}", setAbsolute);
        }

        private void SetPlaylistExtension(PlaylistExtension extension)
        {
            switch (extension)
            {
                case PlaylistExtension.M3U:
                    _playlistExtension = "m3u";
                    break;
                case PlaylistExtension.PLS:
                    _playlistExtension = "pls";
                    break;
                case PlaylistExtension.XSPF:
                    _playlistExtension = "xspf";
                    break;
            }
        }

        private void OnClosePressed(Window window)
        {
            window.Close();
        }
        private void FindExtensions()
        {
            foreach (Format f in PlaylistIOFactory.GetInstance().getFormats())
            {
                if (f.Readable)
                {
                    foreach (string extension in f)
                    {
                        _playlistExtensions.Add(extension.Trim('.'));
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
                        Source = _mediaIconModel.FolderImage
                    };
                    itemList.Add(new MediaItemModel(dir, MediaItemType.Directory, mediaImage));
                }
            }

            foreach (FileInfo file in info.GetFiles().OrderBy(file => file.Name))
            {
                if (_playlistExtensions.Any(file.Extension.Contains))
                {
                    Image mediaImage = new Image
                    {
                        Source = _mediaIconModel.PlaylistImage
                    };
                    itemList.Add(new MediaItemModel(file, MediaItemType.Playlist, mediaImage));
                }
                else if (_mediaExtensions.Any(file.Extension.Contains))
                {
                    Image mediaImage = new Image
                    {
                        Source = _mediaIconModel.MusicImage
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
                    PlaylistTracks.Add(new PlaylistTrack(new Track(selectedItem.FullPath)));
                    _playlistMedia.Add(new(new Media(_libVlc, new Uri(selectedItem.FullPath))));
                    UpdatePlaylistTotals();
                    Log.Information("Adding {Arg0} tracks to the playlist", PlaylistTracks.Count);
                    break;
                }
                case MediaItemType.Playlist:
                {
                    FileInfo file = new FileInfo(selectedItem.FullPath);
                    try
                    {
                        IPlaylist playlist = PlaylistHandler(file);
                        ImportPlaylist(playlist);
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
        private void NewPlaylist()
        {
            if (_playlistMedia.Count > 0)
            {
                MediaPlayback(PlaybackControl.Stop);
            }
            NowPlaying(false);
            PlaylistTracks.Clear();
            _playlistMedia.Clear();
            UpdatePlaylistTotals();
        }

        private void ImportPlaylist(IPlaylist playlist)
        {
            if (_playlistMedia.Count > 0)
            {
                MediaPlayback(PlaybackControl.Stop);
            }
            PlaylistTracks.Clear();
            NowPlaying(false);
            foreach (Track track in playlist.ReadList)
            {
                PlaylistTracks.Add(new PlaylistTrack(track));
                _playlistMedia.Add(new(new Media(_libVlc, new Uri(track.Path))));
            }
            UpdatePlaylistTotals();
        }

        private async Task SavePlaylist()
        {
            SaveFileDialog dialog = new();
            dialog.Filters.Add(new FileDialogFilter{Name = $"Playlists ({_playlistExtension})", Extensions = _playlistExtensions});
            dialog.Title = $"Save Playlist as {_playlistExtension}";
            dialog.DefaultExtension = _playlistExtension;
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
                    playlist.Relative = !_playlistAbsolute;
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
            PlaylistTotals = $"{totalTracks} tracks - [{TimeSpan.FromSeconds(totalTime)}]";
        }

        private void MovePlaylistItem(bool moveUp)
        {
            int index = SelectedPlaylistIndex;
            try
            {
                if (moveUp)
                {
                    MoveListItem(PlaylistTracks, index, index - 1);
                    MoveListItem(_playlistMedia, index, index - 1);
                    SelectedPlaylistIndex = index - 1;
                }
                else
                {
                    MoveListItem(PlaylistTracks, index, index + 1);
                    MoveListItem(_playlistMedia, index, index + 1);
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
            _playlistMedia.RemoveAt(trackIndex);
            UpdatePlaylistTotals();
            Log.Information("Removing '{Arg0}' from playlist", track.Title);
        }

        private void MediaPlayback(PlaybackControl control)
        {
            switch (control)
            {
                case PlaybackControl.Play:
                {
                    foreach (MediaPlayer media in _playlistMedia)
                    {
                        media.Stop();
                    }
                    _playlistMedia[SelectedPlaylistIndex].Play();
                    NowPlaying(true);
                    break;
                }
                case PlaybackControl.Pause:
                {
                    _playlistMedia[SelectedPlaylistIndex].Pause();
                    break;
                }
                case PlaybackControl.Stop:
                {
                    _playlistMedia[SelectedPlaylistIndex].Stop();
                    NowPlaying(false);
                    break;
                }
                case PlaybackControl.Next:
                {
                    try
                    {
                        _playlistMedia[SelectedPlaylistIndex].Stop();
                        SelectedPlaylistIndex += 1;
                        _playlistMedia[SelectedPlaylistIndex].Play();
                        NowPlaying(true);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("End of playlist");
                    }
                    break;
                }
                case PlaybackControl.Previous:
                {
                    try
                    {
                        _playlistMedia[SelectedPlaylistIndex].Stop();
                        SelectedPlaylistIndex -= 1;
                        _playlistMedia[SelectedPlaylistIndex].Play();
                        NowPlaying(true);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Beginning of playlist");
                    }
                    break;
                }
            }
        }

        private void NowPlaying(bool show)
        {
            if (show)
            {
                Track track = PlaylistTracks[SelectedPlaylistIndex].Track;
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
                    TrackImage.Source = _mediaIconModel.CDImage;
                }
                TrackAlbum = track.Album;
                TrackArtist = track.Artist;
                TrackTitle = track.Title;
                Log.Information("Now playing {Arg0}", track.Title);
            }
            else
            {
                TrackImage.Source = _mediaIconModel.CDImage;
                TrackAlbum = "No Media";
                TrackArtist = "No Media";
                TrackTitle = "No Media";
            }
        }
    }
}