using UnityEngine;

namespace DegasiPilot.UniPooling
{
	class SmartComponentPool<T> : SmartPool<T> where T : Component
	{
		internal SmartComponentPool(IUnityObjectPool<T> pool) : base(pool)
		{
		}

		internal SmartComponentPool(T prefab, int capacity) : base(prefab, capacity)
		{
		}

		internal override T Get()
		{
			var component = base.Get();
			component.gameObject.SetActive(true);
			return component;
		}

		protected override void Release(T obj)
		{
			obj.gameObject.SetActive(false);
			base.Release(obj);
		}
	}
}