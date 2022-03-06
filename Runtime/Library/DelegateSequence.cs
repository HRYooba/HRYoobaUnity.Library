using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace HRYooba.Library
{
    /// <summary>
    /// CallbackにonCompleteを持つデリゲート
    /// </summary>
    /// <param name="onComplete">コンプリート処理が入るデリゲート</param>
    public delegate void Process(System.Action onComplete);

    /// <summary>
    /// デリゲートのCallbackでシーケンスを作るクラス
    /// </summary>
    public class DelegateSequence
    {
        private List<Process> _processList = default;
        private System.Action _onComplete = default;

        public DelegateSequence()
        {
            _processList = new List<Process>();
        }

        ~DelegateSequence() { }

        public DelegateSequence Append(Process process)
        {
            _processList.Add(process);
            return this;
        }

        public DelegateSequence Play()
        {
            Observable.FromCoroutine<Unit>(observer => NextProcessCoroutine(observer)).Subscribe();
            return this;
        }

        public DelegateSequence OnComplete(System.Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        private IEnumerator NextProcessCoroutine(System.IObserver<Unit> observer)
        {
            bool canNext = false;
            System.Action onComplete = () => canNext = true;

            foreach (var process in _processList)
            {
                canNext = false;
                process?.Invoke(onComplete);
                yield return new WaitUntil(() => canNext);
            }

            // OnComplete
            _onComplete?.Invoke();
            observer.OnCompleted();

            // Clear
            _processList.Clear();
            _processList = null;
            _onComplete = null;
        }
    }
}