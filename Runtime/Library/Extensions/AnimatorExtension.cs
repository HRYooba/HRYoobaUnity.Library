using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HRYooba.Library
{
    public static class AnimatorExtension
    {
        public static void ResetAllTrigger(this Animator animator)
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(parameter.name);
                }
            }
        }

        public static void SetTriggerOneFrame(this Animator animator, string name)
        {
            SetTriggerOnFrameAsync(animator, name, animator.GetCancellationTokenOnDestroy()).Forget();
        }

        private static async UniTask SetTriggerOnFrameAsync(this Animator animator, string name, CancellationToken cancellationToken)
        {
            animator.SetTrigger(name);
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            if (animator != null) animator.ResetTrigger(name);
        }

        public static async UniTask PlayTriggerAnimationAsync(this Animator animator, string name, CancellationToken cancellationToken)
        {
            await SetTriggerOnFrameAsync(animator, name, cancellationToken);
            var fullPathHash = animator.GetNextAnimatorStateInfo(0).fullPathHash;
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).fullPathHash == fullPathHash, cancellationToken: cancellationToken);
            await UniTask.WaitUntil(() => !(animator.GetCurrentAnimatorStateInfo(0).fullPathHash == fullPathHash), cancellationToken: cancellationToken);
        }

        public static async UniTask CrossFadeInFixedTimeAsync(this Animator animator, string stateName, float fixedTransitionDuration, CancellationToken cancellationToken)
        {
            animator.CrossFadeInFixedTime(stateName, fixedTransitionDuration);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName), cancellationToken: cancellationToken);
            await UniTask.WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(stateName), cancellationToken: cancellationToken);
        }
    }
}