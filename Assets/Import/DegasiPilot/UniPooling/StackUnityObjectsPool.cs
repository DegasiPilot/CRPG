using System.Collections.Generic;

namespace DegasiPilot.UniPooling
{
	internal class StackUnityObjectsPool<T> : IUnityObjectPool<T> where T : UnityEngine.Object
	{
		private T _prefab;
		private Stack<T> _pooledObjects;

		public StackUnityObjectsPool(T prefab, int capacity, bool isLazy = true)
		{
			_prefab = prefab;
			_pooledObjects = new(capacity);
			if (!isLazy)
			{
				for(int i = 0; i < capacity; i++)
				{
					_pooledObjects.Push(UnityEngine.Object.Instantiate(_prefab));
				}
			}
		}

		public PooledUnityObject<T> Get()
		{
			PooledUnityObject<T> pooledObject = null;
			while (pooledObject == null)
			{
				if (_pooledObjects.TryPop(out T innerObject))
				{
					continue;
				}
				else
				{
					return new PooledUnityObject<T>(UnityEngine.Object.Instantiate(_prefab), this);
				}
			}
			return pooledObject;
		}

		public void Release(T unityObject)
		{
			_pooledObjects.Push(unityObject);
		}
	}
}