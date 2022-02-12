using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using ATL;
using ATL.AudioData;
using ATL.Playlist;
using Avalonia.Controls;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.Views;
using ReactiveUI;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string _musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private string _currentDirectory = "";
        private string _rootDirectory = Directory.GetDirectoryRoot(Environment.SpecialFolder.Personal.ToString());
        private int _selectedIndex;
        private readonly List<string> _playlistExtensions= new();
        private readonly List<string> _mediaExtensions = new();
        private IconModel _mediaIconModel;
        private List<MediaItemModel> _itemList = new();

        private Stack<string> _undoStack = new();

        private Stack<string> _redoStack = new();
        private ObservableCollection<PlaylistTabModel> _playlistTabModels = new();
        private int _playlistItem;
        public List<MediaItemModel> ItemList
        {
            get => _itemList; 
            private set => this.RaiseAndSetIfChanged(ref _itemList, value);
        }
        public IconModel MediaIconModel;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }
        public ObservableCollection<PlaylistTabModel> PlaylistTabModels
        {
            get => _playlistTabModels;
            set => this.RaiseAndSetIfChanged(ref _playlistTabModels, value);
        }

        public int PlaylistItem
        {
            get => _playlistItem;
            set => this.RaiseAndSetIfChanged(ref _playlistItem, value);
        }
        public ReactiveCommand<Unit, Unit> HomeBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> ParentBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> UndoBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> RedoBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> NewBtnPressed { get; }
        public ReactiveCommand<Unit, Unit> CloseBtnPressed { get; }

        public MainWindowViewModel()
        {
            _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel));
            FindExtensions();
            ItemList = new List<MediaItemModel>(PopulateTree(_musicDirectory));
            HomeBtnPressed = ReactiveCommand.Create(HomeDirectory);
            ParentBtnPressed = ReactiveCommand.Create(ParentDirectory);
            UndoBtnPressed = ReactiveCommand.Create(UndoNavigation);
            RedoBtnPressed = ReactiveCommand.Create(RedoNavigation);
            NewBtnPressed = ReactiveCommand.Create(NewPlaylist);
            CloseBtnPressed = ReactiveCommand.Create(ClosePlaylist);
        }
        private void FindExtensions()
        {
            foreach (Format f in PlaylistIOFactory.GetInstance().getFormats())
            {
                if (f.Readable)
                {
                    foreach (string extension in f)
                    {
                        _playlistExtensions.Add(extension);
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
            MediaItemModel selectedItem = _itemList[_selectedIndex];
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
                    //TODO: Logic for adding media to playlists
                    break;
                }
                case MediaItemType.Playlist:
                {
                    //TODO: Logic for adding new playlist as new tab
                    break;
                }
            }
        }
        private void NewPlaylist()
        {
            PlaylistTabModels.Add(new PlaylistTabModel($"Playlist {PlaylistTabModels.Count + 1}", new PlaylistDataGrid()));
        }

        private void ClosePlaylist()
        {
            PlaylistTabModels.RemoveAt(PlaylistItem);
        }
    }
}