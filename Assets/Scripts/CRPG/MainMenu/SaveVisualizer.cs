using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveVisualizer : MonoBehaviour
{
    public Text SaveTimeText;
    public Text PersonageInfoText;

    public void Setup(GameSaveInfo saveInfo)
    {
        System.DateTime dateTime = saveInfo.DateTime.ToLocalTime();
        SaveTimeText.text = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
        PersonageInfoText.text = CRUD.GetPersonageInfo(saveInfo.MainPersonageId).Name;
        GetComponentInParent<Button>().onClick.AddListener(() => MainMenuScript.Instance.LoadGame(saveInfo));
    }
}
