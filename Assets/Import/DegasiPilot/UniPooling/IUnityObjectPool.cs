namespace DegasiPilot.UniPooling
{
	internal interface IUnityObjectPool<T> where T : UnityEngine.Object
	{
		PooledUnityObject<T> Get();
		void Release(T unityObject);
	}
}