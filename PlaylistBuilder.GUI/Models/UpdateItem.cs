using System.Collections.Generic;

namespace PlaylistBuilder.GUI.Models;

public class UpdateItem
{
    public static void UpdateListItem<T>(IList<T> list, int index, T updatedTrack)
    {
        list.RemoveAt(index);
        list.Insert(index, updatedTrack);
    }
}