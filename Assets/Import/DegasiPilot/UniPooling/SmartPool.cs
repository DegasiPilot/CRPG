using System;
using System.Collections.Generic;

namespace DegasiPilot.UniPooling
{
	internal class SmartPool<T> : IDisposable where T: UnityEngine.Object
	{
		private IUnityObjectPool<T> _pool;
		private List<PooledUnityObject<T>> _usedObjects;

		internal SmartPool(T prefab, int capacity)
		{
			_pool = new StackUnityObjectsPool<T>(prefab, capacity);
			_usedObjects = new List<PooledUnityObject<T>>(capacity);
		}

		/// <summary>
		/// It work as client for pool
		/// </summary>
		internal SmartPool(IUnityObjectPool<T> pool)
		{
			_pool = pool;
			_usedObjects = new List<PooledUnityObject<T>>();
		}

		internal T Get()
		{
			var pooledObject = _pool.Get();
			_usedObjects.Add(pooledObject);
			return pooledObject.Value;
		}

		public void Dispose()
		{
			foreach(var obj in _usedObjects)
			{
				obj.Dispose();
			}
			_usedObjects.Clear();
		}
	}
}