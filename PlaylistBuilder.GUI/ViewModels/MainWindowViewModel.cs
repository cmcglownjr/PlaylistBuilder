using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ATL;
using ATL.AudioData;
using ATL.Playlist;
using Avalonia.Controls;
using PlaylistBuilder.GUI.Models;
using static PlaylistBuilder.GUI.Models.MoveItem;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using ReactiveUI;
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

        private int _selectedDirectoryIndex;
        private int _selectedPlaylistIndex;
        private readonly List<string> _playlistExtensions = new();
        private readonly List<string> _mediaExtensions = new();
        private readonly IconModel? _mediaIconModel;
        private List<MediaItemModel> _itemList = new();
        private Stack<string> _undoStack = new();
        private Stack<string> _redoStack = new();
        private string _playlistDetails = "0 tracks - [00:00:00]";
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

        public MainWindowViewModel()
        {
            _mainWindow = new MainWindow();
            _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel))!;
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
            PlaylistTracks = new();
        }

        private void OpenPreferenceWindow()
        {
            var prefenceWindow = new PreferenceWindow();
            prefenceWindow.Show();
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
            foreach (DirectoryInfo dir in info.GetDirectories())
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

            foreach (FileInfo file in info.GetFiles())
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
                //TODO: Log that you are at the root level
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
                    UpdatePlaylistTotals();
                    break;
                }
                case MediaItemType.Playlist:
                {
                    FileInfo file = new FileInfo(selectedItem.FullPath);
                    IPlaylist playlist = PlaylistHandler(file);
                    ImportPlaylist(playlist);
                    break;
                }
            }
        }
        private void NewPlaylist()
        {
            PlaylistTracks.Clear();
            UpdatePlaylistTotals();
        }

        private void ImportPlaylist(IPlaylist playlist)
        {
            PlaylistTracks.Clear();
            foreach (Track track in playlist.ReadList)
            {
                PlaylistTracks.Add(new PlaylistTrack(track));
            }
            UpdatePlaylistTotals();
        }

        private async Task SavePlaylist()
        {
            SaveFileDialog dialog = new();
            dialog.Filters.Add(new FileDialogFilter{Name = "Playlists", Extensions = _playlistExtensions});
            dialog.Title = "Save Playlist";
            dialog.DefaultExtension = "m3u";
            //TODO: Set up default extensions
            string result = await dialog.ShowAsync(_mainWindow);
            if (result != null)
            {
                IPlaylist playlist = PlaylistHandler(new FileInfo(result));
                foreach (PlaylistTrack playlistTrack in PlaylistTracks)
                {
                    playlist.AddTrack(playlistTrack.Track);
                }
                playlist.SavePlaylist(result);
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
                    IPlaylist playlist = PlaylistHandler(file);
                    ImportPlaylist(playlist);
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
            if (moveUp)
            {
                MoveListItem(PlaylistTracks, SelectedPlaylistIndex, SelectedPlaylistIndex - 1);
                SelectedPlaylistIndex -= 2;
            }
            else
            {
                MoveListItem(PlaylistTracks, SelectedPlaylistIndex, SelectedPlaylistIndex + 1);
                SelectedPlaylistIndex += 1;
            }
        }

        private void RemoveTrack()
        {
            PlaylistTracks.RemoveAt(SelectedPlaylistIndex);
        }
    }
}