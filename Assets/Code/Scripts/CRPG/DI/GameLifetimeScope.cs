using CRPG.DataSaveSystem;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CRPG.DI 
{
	public class GameLifetimeScope : LifetimeScope
	{
		[SerializeField] private Player PlayerPrefab;
		[SerializeField] private RaceInfo[] RaceInfos;
		[SerializeField] private PersonageActionInfo[] PersonageActionInfos;
		[SerializeField] private List<Item> AllItems;

		protected override void Configure(IContainerBuilder builder)
		{
			GameData.RaceInfos = RaceInfos;
			GameData.PersonageActionInfos = PersonageActionInfos;
			builder.RegisterInstance(RaceInfos);
			builder.RegisterInstance(PersonageActionInfos);
			DI.DataSaveLoader = new LocalSaveLoader();
			builder.RegisterInstance<IDataSaveLoader>(DI.DataSaveLoader); //TODO: Refactor
			builder.RegisterComponentInNewPrefab(PlayerPrefab, Lifetime.Singleton).DontDestroyOnLoad();
			builder.Register(
				(IObjectResolver resolver) => resolver.Resolve<Player>().PlayerController,
				Lifetime.Singleton);
			builder.Register(
				(IObjectResolver resolver) =>
				{
					Player player = resolver.Resolve<Player>();
					PlayerCustomizer playerCustomizer = player.PlayerCustomizer;
					playerCustomizer.Setup(player.PlayerController.Personage);
					return playerCustomizer;
				},
				Lifetime.Singleton);
			builder.RegisterBuildCallback(BuildCallback);
		}

		private void BuildCallback(IObjectResolver resolver)
		{
			GameData.Player = resolver.Resolve<Player>();
		}
	}
}
