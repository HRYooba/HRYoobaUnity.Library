using System;
using UnityEngine;
using UniRx;

namespace HRYooba.Library
{
    public static class UniRxUtility
    {
        public static IObservable<Unit> EitherTimerOrUnitObservable(float seconds, IObservable<Unit> unitObservable)
        {
            return Observable.Amb(Observable.Timer(TimeSpan.FromSeconds(seconds)).AsUnitObservable(), unitObservable);
        }
    }
}