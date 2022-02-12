using System;
using System.IO;
using ATL;

namespace PlaylistBuilder.GUI.Models;

public class PlaylistTrack
{
    public Track Track { get; }
    public int TrackNumber { get; }
    public string Title { get; }
    public string Artist { get; }
    public string Album { get; }
    public TimeSpan Duration { get; }
    public string FileName { get; }

    public PlaylistTrack(Track track)
    {
        Track = track;
        if (track.TrackNumber is null)
        {
            TrackNumber = 0;
        }
        else
        {
            TrackNumber = (int)track.TrackNumber;
        }
        Title = track.Title;
        Artist = track.Artist;
        Album = track.Album;
        Duration = TimeSpan.FromSeconds(track.Duration);
        FileName = Path.GetFileName(track.Path);
    }
}