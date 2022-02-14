using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Splat;

namespace PlaylistBuilder.GUI.Models
{
    public class MediaItemModel
    {
        public MediaItemModel(DirectoryInfo directory, MediaItemType mediaItemType, Image mediaIcon)
        {
            FullPath = directory.FullName;
            Name = directory.Name;
            FileType = mediaItemType;
            MediaIcon = mediaIcon;
        }

        public MediaItemModel(FileInfo media, MediaItemType mediaItemType, Image mediaIcon)
        {
            FullPath = media.FullName;
            Name = media.Name;
            Extension = media.Extension;
            FileType = mediaItemType;
            MediaIcon = mediaIcon;
        }
        public string FullPath { get; }
        public string Name { get; }
        public string Extension { get; }
        public MediaItemType FileType { get; }
        public Image MediaIcon { get; }
    }
}