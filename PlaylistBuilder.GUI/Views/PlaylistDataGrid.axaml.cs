using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public class PlaylistDataGrid : UserControl
{
    public PlaylistDataGrid()
    {
        DataContext = new PlaylistDataGridViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}