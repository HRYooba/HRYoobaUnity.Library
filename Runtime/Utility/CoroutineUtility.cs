using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Library
{
    public static class CoroutineUtility
    {
        // 遅延実行
        public static IEnumerator DelayExecuteCoroutine(System.Action callback, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            callback?.Invoke();
        }

        public static IEnumerator OneFrameDelayExecuteCoroutine(System.Action callback)
        {
            yield return null;
            callback?.Invoke();
        }
    }
}
