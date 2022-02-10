using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views
{
    public class DirectoryView : UserControl
    {
        private DirectoryViewModel _directoryViewModel;
        public DirectoryView()
        {
            DataContext = Locator.Current.GetService(typeof(DirectoryViewModel));
            _directoryViewModel = (DirectoryViewModel)Locator.Current.GetService(typeof(DirectoryViewModel));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            _directoryViewModel.SelectedItem();
            //TODO: Do this as an MVVM way
        }
    }
}