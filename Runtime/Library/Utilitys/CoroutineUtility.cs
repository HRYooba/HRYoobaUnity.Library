using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Library
{
    public static class CoroutineUtility
    {
        // 遅延実行
        public static IEnumerator DelayExecuteAction(float delaySeconds, System.Action callback)
        {
            yield return new WaitForSeconds(delaySeconds);
            callback?.Invoke();
        }

        public static IEnumerator DelayExecuteAction(int delayFrame, System.Action callback)
        {
            for (var i = 0; i < delayFrame; i++)
            {
                yield return null;
            }
            callback?.Invoke();
        }

        public static IEnumerator DelayOneFrameExecuteAction(System.Action callback)
        {
            yield return null;
            callback?.Invoke();
        }
    }
}
