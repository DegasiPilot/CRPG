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

		public T Get()
		{
			if (_pooledObjects.TryPop(out T innerObject))
			{
				UnityEngine.Debug.Log("Get pooled object");
				return innerObject;
			}
			else
			{
				UnityEngine.Debug.Log("Instance non puled object");
				return UnityEngine.Object.Instantiate(_prefab);
			}
		}

		public void Release(T unityObject)
		{
			_pooledObjects.Push(unityObject);
		}
	}
}