namespace PlaylistBuilder.GUI.Models
{
    public class FileModel
    {
        public FileModel(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }
        public string FullPath { get; set; }
        public string Name { get; set; }
    }
}