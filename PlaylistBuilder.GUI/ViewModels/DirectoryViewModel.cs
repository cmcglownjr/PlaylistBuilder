using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PlaylistBuilder.GUI.Models;
using ReactiveUI;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class DirectoryViewModel : ViewModelBase
    {
        readonly string _musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public ObservableCollection<object> ItemList { get; }

        public DirectoryViewModel()
        {
            ItemList = new ObservableCollection<object>(PopulateTree(_musicDirectory));
        }
        
        private List<object> PopulateTree(string directory)
        {
            List<string> extensions = new List<string> { ".m3u", ".M3U", ".pls", ".PLS", ".xspf", ".XSPF" };
            List<object> itemList = new List<object>();
            DirectoryInfo info = new DirectoryInfo(directory);
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                itemList.Add(new MediaItemModel(dir, MediaItemType.Directory));
            }

            foreach (FileInfo file in info.GetFiles())
            {
                if (extensions.Any(file.Extension.Contains))
                {
                    itemList.Add(new MediaItemModel(file, MediaItemType.Playlist));
                }
                else
                {
                    itemList.Add(new MediaItemModel(file, MediaItemType.Media));
                }
            }

            return itemList;
        }
    }
}