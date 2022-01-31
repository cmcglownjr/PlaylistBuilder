using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ReactiveUI;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class DirectoryViewModel : ViewModelBase
    {
        string musicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public ObservableCollection<object> TreeList { get; }

        public DirectoryViewModel()
        {
            TreeList = new ObservableCollection<object>(PopulateTree(musicDirectory));
        }
        
        private List<object> PopulateTree(string directory)
        {
            List<object> treeList = new List<object>();
            DirectoryInfo info = new DirectoryInfo(directory);
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                treeList.Add(dir.Name);
            }

            foreach (FileInfo file in info.GetFiles())
            {
                treeList.Add(file.Name);
            }

            return treeList;
        }
    }
}