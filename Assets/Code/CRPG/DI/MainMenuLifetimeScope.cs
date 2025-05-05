using CRPG;
using CRPG.MainMenu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MainMenuLifetimeScope : LifetimeScope
{
    [SerializeField] private MainMenuScript MainMenuScript;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(MainMenuScript);
        builder.Register<GameDataManager>(Lifetime.Singleton);
        builder.Register<MainMenuViewModel>(Lifetime.Singleton);
        builder.RegisterBuildCallback(ResolveViewModels);
    }

    private void ResolveViewModels(IObjectResolver resolver)
    {
        resolver.Resolve<MainMenuViewModel>();
        resolver.Resolve<GameDataManager>();
    }
}