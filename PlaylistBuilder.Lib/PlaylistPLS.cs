using ATL;

namespace PlaylistBuilder.Lib;

public class PlaylistPLS : PlaylistBase
{
    private string _header = "[playlist]";
    protected override void CreateFile()
    {
        int count = 0;
        if (_savedPlaylist.Exists)
        {
            File.Delete(_savedPlaylist.FullName);
        }

        using StreamWriter sw = File.CreateText(_savedPlaylist.FullName);
        sw.WriteLine(_header);
        sw.WriteLine("");
        foreach (Track track in _playlist)
        {
            count++;
            if (_relative)
            {
                if (_savedPlaylist.DirectoryName != null)
                {
                    var path = Path.GetRelativePath(_savedPlaylist.DirectoryName, track.Path);
                    sw.WriteLine($"File{count}={path}");
                }
            }
            else
            {
                sw.WriteLine($"File{count}={track.Path}");
            }
            sw.WriteLine($"Title{count}={track.Title}");
            sw.WriteLine($"Length{count}={track.Duration}");
        }
        sw.WriteLine("");
        sw.WriteLine($"NumberOfEntries={count}");
        sw.WriteLine("Version=2");
    }
}