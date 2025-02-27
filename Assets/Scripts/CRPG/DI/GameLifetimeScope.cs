using CRPG.DataSaveSystem;
using VContainer;
using VContainer.Unity;

namespace CRPG.DI 
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
			DI.DataSaveLoader = new LocalSaveLoader();
			builder.RegisterInstance<IDataSaveLoader>(DI.DataSaveLoader); //TODO: Refactor
		}
    }
}
