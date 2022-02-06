using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using PlaylistBuilder.GUI.Models;
using ReactiveUI;
using Splat;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class DirectoryViewModel : ViewModelBase
    {
        readonly string _musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private IconModel _mediaIconModel;
        public ObservableCollection<MediaItemModel> ItemList { get; }
        public IconModel MediaIconModel;
        public DirectoryViewModel()
        {
            _mediaIconModel = (IconModel)Locator.Current.GetService(typeof(IconModel));
            MediaIconModel = _mediaIconModel;
            ItemList = new ObservableCollection<MediaItemModel>(PopulateTree(_musicDirectory));
        }
        
        private List<MediaItemModel> PopulateTree(string directory)
        {
            List<string> extensions = new List<string> { ".m3u", ".M3U", ".pls", ".PLS", ".xspf", ".XSPF" };
            List<MediaItemModel> itemList = new List<MediaItemModel>();
            DirectoryInfo info = new DirectoryInfo(directory);
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                Image mediaImage = new Image();
                mediaImage.Source = _mediaIconModel.FolderImage;
                itemList.Add(new MediaItemModel(dir, MediaItemType.Directory, mediaImage));
            }

            foreach (FileInfo file in info.GetFiles())
            {
                if (extensions.Any(file.Extension.Contains))
                {
                    Image mediaImage = new Image();
                    mediaImage.Source = _mediaIconModel.PlaylistImage;
                    itemList.Add(new MediaItemModel(file, MediaItemType.Playlist, mediaImage));
                }
                else
                {
                    Image mediaImage = new Image();
                    mediaImage.Source = _mediaIconModel.MusicImage;
                    itemList.Add(new MediaItemModel(file, MediaItemType.Media, mediaImage));
                }
            }
            return itemList;
        }
    }
}