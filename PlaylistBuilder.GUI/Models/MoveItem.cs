using System;
using System.Collections.Generic;

namespace PlaylistBuilder.GUI.Models;

public static class MoveItem
{
    public static void MoveListItem<T>(IList<T> list, int oldIndex, int newIndex)
    {
        var item = list[oldIndex];
        if (newIndex == list.Count || newIndex == -1)
        {
            throw new IndexOutOfRangeException();
        }
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
    }
}