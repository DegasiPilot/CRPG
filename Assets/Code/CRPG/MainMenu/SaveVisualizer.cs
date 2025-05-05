using UnityEngine;
using UnityEngine.UI;
using System;
using CRPG.DataSaveSystem.SaveData;

public class SaveVisualizer : MonoBehaviour
{
    public Text SaveTimeText;
    public Text PersonageInfoText;

	public void Setup(GameSaveInfo saveInfo, Action<GameSaveInfo> loadSave)
    {
        DateTime dateTime = saveInfo.DateTime.ToLocalTime();
        SaveTimeText.text = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
        PersonageInfoText.text = saveInfo.MainPersonageInfo.Name;
        GetComponentInParent<Button>().onClick.AddListener(() => loadSave?.Invoke(saveInfo));
    }
}