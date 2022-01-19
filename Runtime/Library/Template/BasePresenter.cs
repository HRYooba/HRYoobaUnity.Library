using System;
using UnityEngine;

namespace HRYooba.Library
{
    public abstract class BasePresenter<Model, View> : IDisposable
    {
        protected Model _model;
        protected View _view;

        public abstract void Dispose();
    }
}