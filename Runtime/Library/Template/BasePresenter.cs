using System;
using UnityEngine;

namespace HRYooba.Library
{
    public abstract class BasePresenter<Model, View> : IDisposable
    where Model : BaseModel
    where View : MonoBehaviour
    {
        protected Model _model;
        protected View _view;

        public abstract void Dispose();
    }
}