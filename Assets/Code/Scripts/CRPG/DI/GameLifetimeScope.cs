using CRPG.DataSaveSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CRPG.DI 
{
	public class GameLifetimeScope : LifetimeScope
	{
		[SerializeField] private MainPlayer PlayerPrefab;
		[SerializeField] private GlobalDataManager GlobalDataManagerPrefab;

		protected override void Configure(IContainerBuilder builder)
		{
			GlobalDataManager globalDataManager = Instantiate(GlobalDataManagerPrefab);
			DontDestroyOnLoad(globalDataManager);
			builder.RegisterInstance(globalDataManager);
			builder.Register<IDataSaveLoader, LocalSaveLoader>(Lifetime.Singleton);
			GameData.MainPlayer = Instantiate(PlayerPrefab);
			DontDestroyOnLoad(GameData.MainPlayer);
			builder.RegisterInstance(PlayerPrefab);
		}
	}
}