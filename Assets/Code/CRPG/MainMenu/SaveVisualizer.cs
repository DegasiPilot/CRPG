using CRPG.DataSaveSystem.SaveData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveVisualizer : MonoBehaviour
{
	public Text SaveTimeText;
	public Text PersonageInfoText;
	public RawImage Portrait;

	public void Setup(GameSaveInfo saveInfo, Action<GameSaveInfo> loadSave)
	{
		DateTime dateTime = saveInfo.DateTime.ToLocalTime();
		SaveTimeText.text = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
		PersonageInfoText.text = saveInfo.MainPersonageInfo.Name;
		var portraitImage = new Texture2D(1,1);
		portraitImage.LoadImage(saveInfo.MainPersonageInfo.ImageBytes);
		Portrait.texture = portraitImage;
		GetComponentInParent<Button>().onClick.AddListener(() => loadSave?.Invoke(saveInfo));
	}
}