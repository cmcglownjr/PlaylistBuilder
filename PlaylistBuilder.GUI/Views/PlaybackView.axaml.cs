using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public partial class PlaybackView : UserControl
{
    public PlaybackView()
    {
        DataContext = Locator.Current.GetService(typeof(PlaybackViewModel));
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}