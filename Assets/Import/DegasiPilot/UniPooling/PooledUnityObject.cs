using System;

namespace DegasiPilot.UniPooling
{
	internal class PooledUnityObject<T> : IDisposable where T : UnityEngine.Object
	{
		public PooledUnityObject(T value, IUnityObjectPool<T> myPool)
		{
			Value = value;
			MyPool = myPool;
		}

		public PooledUnityObject(T value, IUnityObjectPool<T> myPool, UnityEngine.GameObject owner)
		{
			Value = value;
			MyPool = myPool;
		}

		public T Value { get; }
		public IUnityObjectPool<T> MyPool { get; }

		public void Dispose()
		{
			MyPool.Release(Value);
		}
	}
}