namespace DegasiPilot.UniPooling
{
	internal interface IUnityObjectPool<T> where T : UnityEngine.Object
	{
		T Get();
		void Release(T unityObject);
	}
}