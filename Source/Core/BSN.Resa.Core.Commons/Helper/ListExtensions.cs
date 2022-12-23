using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.Core.Commons
{
    public static class ListExtensions
    {
        public static void Modify<T>(this IList<T> currentList, IList<T> list)
        {
            var added = list.Except(currentList).ToList();
            var removed = currentList.Except(list).ToList();

            removed.ForEach(a => currentList.Remove(a));
            added.ForEach(a => currentList.Add(a));
        }
    }
}