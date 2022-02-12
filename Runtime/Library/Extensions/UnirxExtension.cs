using System;
using UnityEngine;
using UniRx;

namespace HRYooba.Library
{
    public static class UnirxExtension
    {
        public static IObservable<Unit> EitherTimerOrUnitObservable(this IObservable<Unit> self, float time, IObservable<Unit> unitObservable)
        {
            return Observable.Amb(Observable.Timer(TimeSpan.FromSeconds(60.0f)).AsUnitObservable(), unitObservable);
        }
    }
}