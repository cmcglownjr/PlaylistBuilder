using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public partial class PlaylistView : UserControl
{
    private PlaylistViewModel _playlistViewModel;
    public PlaylistView()
    {
        DataContext = Locator.Current.GetService(typeof(PlaylistViewModel));
        _playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        _playlistViewModel.DblTappedPlaylist();
    }
}