using VContainer;
using VContainer.Unity;
using UnityEngine;
using CRPG.PersonageRedactor;

public class PersonageRedactorLifetimeScope : LifetimeScope
{
    [SerializeField] private PersonageCreator PersonageCreator;

	protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(PersonageCreator);
        builder.Register<PersonageCreatorViewModel>(Lifetime.Singleton);
        builder.RegisterBuildCallback(BuildCallback);
    }

    private void BuildCallback(IObjectResolver resolver)
    {
        resolver.Resolve<PersonageCreatorViewModel>();
    }
}
