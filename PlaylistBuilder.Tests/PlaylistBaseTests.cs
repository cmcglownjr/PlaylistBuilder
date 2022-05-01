using System;
using System.Linq;
using ATL;
using Moq;
using PlaylistBuilder.Lib;
using Xunit;

namespace PlaylistBuilder.Tests;

public class PlaylistBaseTests
{
    [Fact]
    public void TestRemoveDuplicates()
    {
        // The RemoveDuplicates function only removes duplicates based on file paths.
        // Arrange
        var mockTrack1 = new Track("/path/to/track_1")
        {
            Album = "Album 1",
            Title = "Title 1",
            Artist = "Artist 1",
            TrackNumber = 1
        };
        var mockTrack2 = new Track("/path/to/track_2")
        {
            Album = "Album 2",
            Title = "Title 2",
            Artist = "Artist 2",
            TrackNumber = 1
        };
        var mockTrack3 = new Track("/path/to/track_1")
        {
            Album = "Album 1",
            Title = "Title 1",
            Artist = "Artist 1",
            TrackNumber = 1
        };
        IPlaylist testPlaylist = new PlaylistM3U();
        testPlaylist.AddTrack(mockTrack1);
        testPlaylist.AddTrack(mockTrack2);
        testPlaylist.AddTrack(mockTrack3);
        // Act
        testPlaylist.RemoveDuplicates();
        var dup = from x in testPlaylist.ReadList
            group x by x.Path
            into y
            let count = y.Count()
            orderby count descending
            select new { Name = y.Key, Count = count };
        // Assert
        foreach (var x in dup)
        {
            Assert.Equal(1, x.Count);
        }
    }
}