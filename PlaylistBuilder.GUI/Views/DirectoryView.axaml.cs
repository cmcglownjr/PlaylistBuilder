using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaylistBuilder.GUI.ViewModels;
using Splat;

namespace PlaylistBuilder.GUI.Views
{
    public class DirectoryView : UserControl
    {
        public DirectoryView()
        {
            DataContext = Locator.Current.GetService(typeof(DirectoryViewModel));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}