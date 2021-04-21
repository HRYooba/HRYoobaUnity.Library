using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    // 配列データのデバッグ時の確認用にストリング変換
    public static string ToResult<TSource>(this IEnumerable<TSource> source)
    {
        return "{" + string.Join(", ", source) + "}";
    }
}