using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HRYooba.Library
{
    public class ObjectPool<T> : IDisposable where T : PooledObjectBase
    {
        private readonly List<T> _pooledObjects = new();
        public IReadOnlyList<T> AllPooledObjects => _pooledObjects;

        public ObjectPool(T prefab, uint capacity, Transform parent, int layer = -1)
        {
            Increase(prefab, capacity, parent, layer);
        }

        public void Increase(T prefab, uint count, Transform parent, int layer = -1)
        {
            for (var i = 0; i < count; i++)
            {
                Increase(prefab, parent, layer);
            }
        }

        public void Increase(T prefab, Transform parent, int layer = -1)
        {
            var pooledObject = GameObject.Instantiate(prefab, parent);
            pooledObject.name = $"[{_pooledObjects.Count}] {prefab.name}";

            if (layer > 0) pooledObject.gameObject.layer = layer;

            pooledObject.Initialize();
            pooledObject.Deactivate(true);
            _pooledObjects.Add(pooledObject);
        }

        public T Get(bool isRandom = false)
        {
            var pooledObject = isRandom
                                ? _pooledObjects.Where(p => !p.IsActive).OrderBy(_ => Guid.NewGuid()).FirstOrDefault()
                                : _pooledObjects.FirstOrDefault(p => !p.IsActive);

            pooledObject?.Activate();
            return pooledObject;
        }

        public bool TryGet(out T pooledObject, bool isRandom = false)
        {
            pooledObject = Get(isRandom);
            return pooledObject != null;
        }

        public void Release(T pooledObject, bool isForce = false)
        {
            if (_pooledObjects.Contains(pooledObject))
            {
                pooledObject?.Deactivate(isForce);
            }
        }

        public void ReleaseAll(bool isForce = false)
        {
            foreach (var pooledObject in _pooledObjects)
            {
                pooledObject?.Deactivate(isForce);
            }
        }

        public void Dispose()
        {
            foreach (var pooledObject in _pooledObjects)
            {
                GameObject.Destroy(pooledObject.gameObject);
            }
            _pooledObjects.Clear();
        }
    }
}
