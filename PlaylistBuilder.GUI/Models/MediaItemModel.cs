using System.IO;

namespace PlaylistBuilder.GUI.Models
{
    
    public class MediaItemModel
    {
        public MediaItemModel(DirectoryInfo directory, MediaItemType mediaItemType)
        {
            FullPath = directory.FullName;
            Name = directory.Name;
            FileType = mediaItemType;
        }

        public MediaItemModel(FileInfo media, MediaItemType mediaItemType)
        {
            FullPath = media.FullName;
            Name = media.Name;
            FileType = mediaItemType;
        }
        public string FullPath { get; }
        public string Name { get; }
        public MediaItemType FileType { get; }
    }
}