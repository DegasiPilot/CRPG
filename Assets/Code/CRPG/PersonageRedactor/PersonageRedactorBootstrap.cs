using UnityEngine;
using VContainer.Unity;
using VContainer;
namespace CRPG.PersonageRedactor
{
    class PersonageRedactorBootstrap : MonoBehaviour
    {
        [SerializeField] private LifetimeScope Scope;

		private void Awake()
		{
			var container = Scope.Container;
			PersonageCreatorViewModel viewModel = container.Resolve<PersonageCreatorViewModel>();
		}
	}
}
