using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PlaylistBuilder.GUI.Models
{
    public class IconModel
    {
        public readonly Bitmap FolderImage, MusicImage, PlaylistImage;

        public readonly Bitmap? CDImage;
        public IconModel()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            FolderImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/folder.png")));
            MusicImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/music.png")));
            PlaylistImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/playlist.png")));
            CDImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/CD.png")));
        }
    }
}