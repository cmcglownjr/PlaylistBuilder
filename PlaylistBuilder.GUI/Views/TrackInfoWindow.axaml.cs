using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public partial class TrackInfoWindow : Window
{
    public TrackInfoWindow()
    {
        DataContext = Locator.Current.GetService(typeof(TrackInfoViewModel));
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