using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HRYooba.Library
{
    public static class AnimationExtension
    {
        public static async UniTask PlayAsync(this Animation animation, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => animation?.Stop());

            animation.Play();
            await UniTask.WaitUntil(() => !animation.isPlaying, cancellationToken: cancellationToken);
        }

        public static async UniTask PlayAsync(this Animation animation, string clipName, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => animation?.Stop(clipName));

            animation.Play(clipName);
            await UniTask.WaitUntil(() => !animation.isPlaying, cancellationToken: cancellationToken);
        }
    }
}