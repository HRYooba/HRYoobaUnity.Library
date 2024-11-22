using UnityEngine;

namespace HRYooba.Library
{
    public abstract class PooledObjectBase : MonoBehaviour
    {
        protected internal abstract bool IsActive { get; }
        protected internal abstract void Initialize();
        protected internal abstract void Activate();
        protected internal abstract void Deactivate(bool isForce);
    }
}
