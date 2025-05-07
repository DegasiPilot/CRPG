using System;
using System.Collections.Generic;

namespace DegasiPilot.UniPooling
{
	internal class SmartPool<T> : IDisposable where T: UnityEngine.Object
	{
		private IUnityObjectPool<T> _pool;
		private List<T> _usedObjects;

		internal SmartPool(T prefab, int capacity)
		{
			_pool = new StackUnityObjectsPool<T>(prefab, capacity);
			_usedObjects = new List<T>(capacity);
		}

		/// <summary>
		/// It work as client for pool
		/// </summary>
		internal SmartPool(IUnityObjectPool<T> pool)
		{
			_pool = pool;
			_usedObjects = new List<T>();
		}

		internal virtual T Get()
		{
			var pooledObject = _pool.Get();
			_usedObjects.Add(pooledObject);
			return pooledObject;
		}

		public void Dispose()
		{
			foreach(var obj in _usedObjects)
			{
				Release(obj);
			}
			_usedObjects.Clear();
		}

		protected virtual void Release(T obj)
		{
			_pool.Release(obj);
		}
	}
}