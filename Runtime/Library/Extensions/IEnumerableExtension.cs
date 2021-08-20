using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtension
{
    // 配列データのデバッグ時の確認用にストリング変換
    public static string ToResult<T>(this IEnumerable<T> source)
    {
        return "{" + string.Join(", ", source) + "}";
    }

    // 関数実行
    public static IEnumerable<T> Do<T>(this IEnumerable<T> source, System.Action<T> action)
    {
        return source;
    }
}