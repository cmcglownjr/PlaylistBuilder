using System;
using System.Drawing;
using System.IO;

namespace PlaylistBuilder.GUI.ViewModels
{
    public class DirectoryViewModel
    {
        private string fileName = Path.GetFileNameWithoutExtension(
            @"/home/eezyville/Programming/Rider/PlaylistBuilder/Sandbox/07 - Breath of the Wild- Champion Medley.ogg");
        public string Name
        {
            get;
        }
    }
}