using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveVisualizer : MonoBehaviour
{
    public Text SaveTimeText;
    public Text PersonageInfoText;

	public void Setup(GameSaveInfo saveInfo, Action<GameSaveInfo> loadSave)
    {
        System.DateTime dateTime = saveInfo.DateTime.ToLocalTime();
        SaveTimeText.text = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
        PersonageInfoText.text = saveInfo.MainPersonageInfo.Name;
        GetComponentInParent<Button>().onClick.AddListener(() => loadSave?.Invoke(saveInfo));
    }
}