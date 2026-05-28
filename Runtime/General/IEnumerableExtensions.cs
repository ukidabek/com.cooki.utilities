using System;
using System.Collections.Generic;

namespace Utilities.General
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
            return enumerable;
        }
    }
}