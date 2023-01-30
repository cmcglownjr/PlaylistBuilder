using System;
using System.Collections.Generic;
using System.Linq;
using ATL;
using DynamicData;
using Moq;
using PlaylistBuilder.Lib;
using Xunit;

namespace PlaylistBuilder.Tests;

public class PlaylistBaseTests
{
    private Track mockTrack1 = new Track("/path/to/track_1")
    {
        Album = "Album 1",
        Title = "Title 1",
        Artist = "Artist 1",
        TrackNumber = 1
    };
    private Track mockTrack2 = new Track("/path/to/track_2")
    {
        Album = "Album 2",
        Title = "Title 2",
        Artist = "Artist 2",
        TrackNumber = 1
    };
    private Track mockTrack3 = new Track("/path/to/track_1")
    {
        Album = "Album 1",
        Title = "Title 1",
        Artist = "Artist 1",
        TrackNumber = 1
    };
    private Track mockTrack4 = new Track("/path/to/track_3")
    {
        Album = "Album 3",
        Title = "Title 3",
        Artist = "Artist 3",
        TrackNumber = 1
    };
    private Track mockTrack5 = new Track("/path/to/track_4")
    {
        Album = "Album 4",
        Title = "Title 4",
        Artist = "Artist 4",
        TrackNumber = 1
    };
    [Fact]
    public void TestRemoveDuplicates()
    {
        // The RemoveDuplicates function only removes duplicates based on file paths.
        // Arrange
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

    [Fact]
    public void TestRemoveUnavailable()
    {
        // The RemoveUnavailable method removes unavailable tracks based on filepath
        // Arrange
        IPlaylist testPlaylist = new PlaylistM3U();
        testPlaylist.AddTrack(mockTrack1);
        testPlaylist.AddTrack(mockTrack2);
        testPlaylist.AddTrack(mockTrack3);
        // Act
        testPlaylist.RemoveUnavailable();
        var dup = from x in testPlaylist.ReadList
            group x by x.Path
            into y
            let count = y.Count()
            orderby count descending
            select new { Name = y.Key, Count = count };
        // Assert
        foreach (var x in dup)
        {
            Assert.Equal(0, x.Count);
        }
    }

    [Fact]
    public void TestShuffle()
    {
        // The Shuffle method should reorder the existing tracks
        // Arrange
        IPlaylist testPlaylist = new PlaylistM3U();
        testPlaylist.AddTrack(mockTrack1);
        testPlaylist.AddTrack(mockTrack2);
        testPlaylist.AddTrack(mockTrack4);
        testPlaylist.AddTrack(mockTrack5);
        // Act
        // Clone the list
        var originalPlaylist = new List<Track>(testPlaylist.ReadList);
        testPlaylist.ShufflePlaylist();
        var shuffledPlaylist = testPlaylist.ReadList;
        // Assert
        Assert.NotEqual(originalPlaylist[0].Path, shuffledPlaylist[0].Path);

    }
}