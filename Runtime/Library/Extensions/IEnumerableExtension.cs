using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtension
{
    public static string ToResult<T>(this IEnumerable<T> source)
    {
        return "{" + string.Join(", ", source) + "}";
    }

    public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}