using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PlaylistBuilder.GUI.Models
{
    public class IconModel
    {
        public readonly Bitmap FolderImage = 
            new(AssetLoader.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/folder.png"))), 
            MusicImage = new(AssetLoader.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/music.png"))), 
            PlaylistImage = new(AssetLoader.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/playlist.png")));
        public readonly Bitmap? CDImage = 
            new(AssetLoader.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/CD.png")));
    }
}