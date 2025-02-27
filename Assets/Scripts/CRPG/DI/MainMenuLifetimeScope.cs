using CRPG;
using CRPG.ViewModels;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MainMenuLifetimeScope : LifetimeScope
{
    [SerializeField]
    private MainMenuScript MainMenuScript;
	[SerializeField]
	private GameDataManager GameDataManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(MainMenuScript);
        builder.RegisterComponent(GameDataManager);
        builder.RegisterEntryPoint<MainMenuViewModel>();
    }
}