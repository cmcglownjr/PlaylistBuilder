using System;

namespace PlaylistBuilder.GUI.Models;

public class PlaylistTrack
{
    public int Track { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public TimeSpan Duration { get; set; }
    public string FileName { get; set; }
}