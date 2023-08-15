using System;

namespace PlaylistBuilder.GUI.Models;

public class BreadcrumbModel
{
    private string _text;

    public string Text
    {
        get => Truncate(_text, 8);
        set => _text = value;
    }

    public string Path { get; set; }

    public BreadcrumbModel(string text, string path)
    {
        Text = text;
        Path = path;
    }
    private static string Truncate(string s, int max) 
    { 
        return s?.Length > max ? s.Substring(0, max) : s ?? throw new ArgumentNullException(s); 
    }
}