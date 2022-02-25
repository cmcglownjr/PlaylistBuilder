using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PlaylistBuilder.GUI.Models
{
    public class IconModel
    {
        public readonly Bitmap FolderImage, MusicImage, PlaylistImage, RedoImage, UndoImage, ParentImage, HomeImage, 
            PreviousImage, NextImage, PlayImage, PauseImage, StopImage, NewImage, OpenImage, SaveImage;

        public readonly Bitmap? CDImage;

        public readonly Bitmap CancelImage, SwapUpImage, SwapDownImage;

        public IconModel()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            FolderImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/folder.png")));
            MusicImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/music.png")));
            PlaylistImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/playlist.png")));
            RedoImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/redo.png")));
            UndoImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/undo.png")));
            ParentImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/parent.png")));
            HomeImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/home.png")));
            PreviousImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/previous.png")));
            NextImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/next.png")));
            PlayImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/play.png")));
            PauseImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/pause.png")));
            StopImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/stop.png")));
            NewImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/new.png")));
            OpenImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/open.png")));
            SaveImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/save.png")));
            CDImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/CD.png")));
            CancelImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/cancel.png")));
            SwapUpImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/swapUp.png")));
            SwapDownImage = new Bitmap(assets.Open(new Uri("avares://PlaylistBuilder.GUI/Assets/swapDown.png")));
        }
    }
}