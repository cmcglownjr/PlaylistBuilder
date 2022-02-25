using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.Models;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public partial class TrackInfoWindow : Window
{
    public TrackInfoWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    public TrackInfoWindow(PlaylistTrack track)
    {
        DataContext = new TrackInfoViewModel(track);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}