using CRPG;
using CRPG.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VContainer.Unity;

namespace CRPG.MainMenu
{
	internal class MainMenuBootstrap : IStartable
	{
        readonly MainMenuViewModel _mainMenuViewModel;

		public MainMenuBootstrap(MainMenuViewModel mainMenuViewModel)
		{
			_mainMenuViewModel = mainMenuViewModel;
		}

		public void Start()
		{

		}
	}
}
