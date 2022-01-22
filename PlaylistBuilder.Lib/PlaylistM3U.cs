using ATL;

namespace PlaylistBuilder.Lib;

public class PlaylistM3U : PlaylistBase
{
    private string _header = "#EXTM3U";
    protected override void CreateFile()
    {
        if (_savedPlaylist.Exists)
        {
            File.Delete(_savedPlaylist.FullName);
        }

        using StreamWriter sw = File.CreateText(_savedPlaylist.FullName);
        sw.WriteLine(_header);
        foreach (Track item in _playlist)
        {
            sw.WriteLine($"#EXTINF:{item.Duration},{item.Artist} - {item.Title}");
            if (_relative)
            {
                if (_savedPlaylist.DirectoryName != null)
                {
                    var path = Path.GetRelativePath(_savedPlaylist.DirectoryName, item.Path);
                    sw.WriteLine($"{path}");
                }
            }
            else
            {
                sw.WriteLine($"{item.Path}");
            }
        }
    }
}