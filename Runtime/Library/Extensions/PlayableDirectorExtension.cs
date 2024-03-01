
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;
using R3;

namespace HRYooba.Library
{
    public static class PlayableDirectorExtension
    {
        public static void RewindPlay(this PlayableDirector playableDirector)
        {
            playableDirector.time = 0.0;
            playableDirector.Play();
        }

        public static void Rewind(this PlayableDirector playableDirector)
        {
            playableDirector.time = 0.0;
            playableDirector.Stop();
        }

        public static async UniTask PlayAsync(this PlayableDirector playableDirector, CancellationToken cancellationToken, bool enablePause = false, bool cancelStop = true)
        {
            if (cancelStop) cancellationToken.Register(() => playableDirector.Stop());

            playableDirector.Play();
            if (enablePause)
            {
                await playableDirector.StoppedAsObservable().Merge(playableDirector.PausedAsObservable()).FirstAsync(cancellationToken);
            }
            else
            {
                await playableDirector.StoppedAsObservable().FirstAsync(cancellationToken);
            }
        }

        public static async UniTask RewindPlayAsync(this PlayableDirector playableDirector, CancellationToken cancellationToken, bool enablePause = false, bool cancelStop = true)
        {
            if (cancelStop) cancellationToken.Register(() => playableDirector.Stop());

            playableDirector.RewindPlay();
            if (enablePause)
            {
                await playableDirector.StoppedAsObservable().Merge(playableDirector.PausedAsObservable()).FirstAsync(cancellationToken);
            }
            else
            {
                await playableDirector.StoppedAsObservable().FirstAsync(cancellationToken);
            }
        }

        public static Observable<PlayableDirector> PlayedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.played += h, h => playableDirector.played -= h);
        }

        public static Observable<PlayableDirector> PausedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.paused += h, h => playableDirector.paused -= h);
        }

        public static Observable<PlayableDirector> StoppedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.stopped += h, h => playableDirector.stopped -= h);
        }
    }
}