using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views;

public partial class NavigatorView : UserControl
{
    private NavigatorViewModel _navigatorViewModel;
    public NavigatorView()
    {
        DataContext = Locator.Current.GetService(typeof(NavigatorViewModel));
        _navigatorViewModel = (NavigatorViewModel)Locator.Current.GetService(typeof(NavigatorViewModel))!;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        _navigatorViewModel.DblTappedItem();
        //TODO: Do this as an MVVM way
    }
}