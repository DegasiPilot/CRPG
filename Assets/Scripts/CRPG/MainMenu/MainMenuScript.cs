using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG
{
	public class MainMenuScript : MonoBehaviour
	{
		public GameObject MainPanel;
		public GameObject SavesPanel;
		public GameObject LoadingPanel;

		public Transform SavesTransformParent;
		public SaveVisualizer SaveSlotPrefab;

		public Button LoadLastGameButton;
		public Button LoadGameButton;
		public Button StartNewGameButton;
		public Button ExitGameButton;

		public void ShowLoadScreen()
		{
			MainPanel.SetActive(false);
			SavesPanel.SetActive(false);
			LoadingPanel.SetActive(true);
		}

		public void ShowSaves(List<GameSaveInfo> saves, Action<GameSaveInfo> loadSave)
		{
			SavesPanel.SetActive(true);
			foreach (var save in saves)
			{
				SaveVisualizer saveVisualizer = Instantiate(SaveSlotPrefab, SavesTransformParent);
				saveVisualizer.Setup(save, loadSave);
			}
		}
	}
}