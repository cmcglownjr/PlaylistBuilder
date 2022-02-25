using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using PlaylistBuilder.GUI.ViewModels;
using PlaylistBuilder.GUI.Views;
using PlaylistBuilder.Lib;
using Serilog;
using Splat;

namespace PlaylistBuilder.GUI.Models;

public class DialogModels
{
    public static async Task SaveFile()
    {
        var mainWindow = new MainWindow();
        PreferenceViewModel preferenceViewModel = (PreferenceViewModel)Locator.Current.GetService(typeof(PreferenceViewModel))!;
        NavigatorViewModel navigatorViewModel = (NavigatorViewModel)Locator.Current.GetService(typeof(NavigatorViewModel))!;
        PlaylistViewModel playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        SaveFileDialog dialog = new();
        dialog.Filters.Add(new FileDialogFilter{Name = $"Playlists ({preferenceViewModel.PlaylistExtension})", 
            Extensions = navigatorViewModel.PlaylistExtensions});
        dialog.Title = $"Save Playlist as {preferenceViewModel.PlaylistExtension}";
        dialog.DefaultExtension = preferenceViewModel.PlaylistExtension;
        string result = await dialog.ShowAsync(mainWindow);
        if (result != null)
        {
            try
            {
                IPlaylist playlist = playlistViewModel.PlaylistHandler(new FileInfo(result));
                foreach (PlaylistTrack playlistTrack in playlistViewModel.PlaylistTracks)
                {
                    playlist.AddTrack(playlistTrack.Track);
                }
                playlist.Relative = !preferenceViewModel.PlaylistAbsolute;
                playlist.SavePlaylist(result);
                Log.Information("Playlist saved to {Arg0}", result);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Information(e,"Imported playlist is not supported");
            }
        }
    }
    public static async Task OpenFile()
    {
        var mainWindow = new MainWindow();
        NavigatorViewModel navigatorViewModel = (NavigatorViewModel)Locator.Current.GetService(typeof(NavigatorViewModel))!;
        PlaylistViewModel playlistViewModel = (PlaylistViewModel)Locator.Current.GetService(typeof(PlaylistViewModel))!;
        OpenFileDialog dialog = new();
        dialog.Filters.Add(new FileDialogFilter{Name = "Playlists", 
            Extensions = navigatorViewModel.PlaylistExtensions});
        dialog.AllowMultiple = false;
        dialog.Title = "Open File";
        string[] result = await dialog.ShowAsync(mainWindow);
        if (result != null)
        {
            foreach (string filepath in result)
            {
                FileInfo file = new(filepath);
                try
                {
                    IPlaylist playlist = playlistViewModel.PlaylistHandler(file);
                    playlistViewModel.ImportPlaylist(playlist);
                    Log.Information("Playlist imported from {Arg0}", result);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.Information(e,"Imported playlist is not supported");
                }
            }
        }
    }
}