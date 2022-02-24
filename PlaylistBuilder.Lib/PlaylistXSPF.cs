using ATL;
using System.Xml;

namespace PlaylistBuilder.Lib;

public class PlaylistXSPF : PlaylistBase
{
    private string? _playlistTitle;
    private string? _playlistCreator;
    protected override void CreateFile()
    {
        var sts = new XmlWriterSettings
        {
            Indent = true,
        };
        using var writer = XmlWriter.Create(_savedPlaylist.FullName, sts);
        writer.WriteStartDocument();
        writer.WriteStartElement("playlist");
        writer.WriteAttributeString("version", "http://xspf.org/ns/0/", "1");
        if (_playlistTitle != null)
        {
            writer.WriteStartElement("title");
            writer.WriteString(_playlistTitle);
            writer.WriteEndElement();
        }

        if (_playlistCreator != null)
        {
            writer.WriteStartElement("creator");
            writer.WriteString(_playlistCreator);
            writer.WriteEndElement();
        }
        
        writer.WriteStartElement("trackList");
        foreach (Track track in Playlist)
        {
            writer.WriteStartElement("track");
            writer.WriteStartElement("title");
            writer.WriteString($"{track.Title}");
            writer.WriteEndElement();
            writer.WriteStartElement("album");
            writer.WriteString($"{track.Album}");
            writer.WriteEndElement();
            writer.WriteStartElement("creator");
            writer.WriteString($"{track.Artist}");
            writer.WriteEndElement();
            writer.WriteStartElement("location");
            if (_relative)
            {
                if (_savedPlaylist.DirectoryName != null)
                {
                    var path = Path.GetRelativePath(_savedPlaylist.DirectoryName, track.Path);
                    writer.WriteString($"file://{path}");
                }
            }
            else
            {
                writer.WriteString($"file://{track.Path}");
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}