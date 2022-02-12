using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;
        public MainWindow()
        {
            DataContext = Locator.Current.GetService(typeof(MainWindowViewModel));
            _mainWindowViewModel = (MainWindowViewModel)Locator.Current.GetService(typeof(MainWindowViewModel));
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.DblTappedItem();
            //TODO: Do this as an MVVM way
        }
    }
}