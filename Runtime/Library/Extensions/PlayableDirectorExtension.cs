using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UniRx;

namespace HRYooba.Library
{
    public static class PlayableDirectorExtension
    {
        public static void BindAllActivationTrackToThisActivation(this PlayableDirector playableDirector)
        {
            var tracks = (playableDirector.playableAsset as TimelineAsset).GetOutputTracks();
            foreach (var track in tracks)
            {
                if (track.GetType() == typeof(ActivationTrack))
                {
                    var activationObject = playableDirector.GetGenericBinding(track) as GameObject;
                    playableDirector.gameObject.ObserveEveryValueChanged(_ => _.activeSelf).Subscribe(activationObject.SetActive).AddTo(playableDirector);
                }
            }
        }
    }
}