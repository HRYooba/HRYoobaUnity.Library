
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UniRx;

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

        public static UniTask PlayAsync(this PlayableDirector playableDirector, CancellationToken cancellationToken, bool enablePause = false, bool cancelStop = true)
        {
            if (cancelStop) cancellationToken.Register(() => playableDirector.Stop());

            playableDirector.Play();
            if (enablePause)
            {
                return playableDirector.StoppedAsObservable().Merge(playableDirector.PausedAsObservable()).ToUniTask(true, cancellationToken);
            }
            else
            {
                return playableDirector.StoppedAsObservable().ToUniTask(true, cancellationToken);
            }
        }

        public static UniTask RewindPlayAsync(this PlayableDirector playableDirector, CancellationToken cancellationToken, bool enablePause = false, bool cancelStop = true)
        {
            if (cancelStop) cancellationToken.Register(() => playableDirector.Stop());

            playableDirector.RewindPlay();
            if (enablePause)
            {
                return playableDirector.StoppedAsObservable().Merge(playableDirector.PausedAsObservable()).ToUniTask(true, cancellationToken);
            }
            else
            {
                return playableDirector.StoppedAsObservable().ToUniTask(true, cancellationToken);
            }
        }

        public static IObservable<PlayableDirector> PlayedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.played += h, h => playableDirector.played -= h);
        }

        public static IObservable<PlayableDirector> PausedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.paused += h, h => playableDirector.paused -= h);
        }

        public static IObservable<PlayableDirector> StoppedAsObservable(this PlayableDirector playableDirector)
        {
            return Observable.FromEvent<PlayableDirector>(h => playableDirector.stopped += h, h => playableDirector.stopped -= h);
        }

        public static void BindAllActivationTrackToThisInactivation(this PlayableDirector playableDirector)
        {
            var tracks = (playableDirector.playableAsset as TimelineAsset).GetOutputTracks();
            foreach (var track in tracks)
            {
                if (track.GetType() == typeof(ActivationTrack))
                {
                    var activationObject = playableDirector.GetGenericBinding(track) as GameObject;
                    playableDirector.gameObject.ObserveEveryValueChanged(_ => _.activeSelf).Where(activeSelf => !activeSelf).Subscribe(activationObject.SetActive).AddTo(playableDirector);
                }
            }
        }
    }
}