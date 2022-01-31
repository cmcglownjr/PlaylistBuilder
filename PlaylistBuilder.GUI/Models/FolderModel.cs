using System.Collections.ObjectModel;

namespace PlaylistBuilder.GUI.Models
{
    public class FolderModel
    {
        public FolderModel(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
            Children = new ObservableCollection<object>();
        }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public bool IsReady { get; set; }
        public ObservableCollection<object> Children { get; private set; }
    }
}