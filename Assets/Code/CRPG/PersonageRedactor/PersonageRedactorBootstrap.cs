using UnityEngine;
using VContainer;
using VContainer.Unity;
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
