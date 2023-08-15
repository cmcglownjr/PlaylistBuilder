using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

using FluentAvalonia.UI.Controls;

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
    
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        _navigatorViewModel.DblTappedItem();
        //TODO: Do this as an MVVM way
    }

    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        _navigatorViewModel.BreadcrumbItemTapped(args.Index);
    }
}