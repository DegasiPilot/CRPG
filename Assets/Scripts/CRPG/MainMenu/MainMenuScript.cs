using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public static MainMenuScript Instance;

    public GameObject MainPanel;
    public GameObject SavesPanel;
    public GameObject LoadingPanel;

    public Transform SavesTransformParent;
    public GameObject SaveSlotPrefab;
    public Button ContinueBtn;
    public Button LoadBtn;
    public GameDataManager GameDataManager;
    public Text UserNameText;

    private List<GameSaveInfo> _saves;

    private void Awake()
    {
        Instance = this;
    }

    public void AfterUserEnter()
    {
        bool hasSaves = CRUD.HasAnySaves(GameData.CurrentUser);
        ContinueBtn.interactable = hasSaves;
        LoadBtn.interactable = hasSaves;
        UserNameText.text = GameData.CurrentUser.Login;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("PersonageRedactorScene");
    }

    public void LoadLastGame()
    {
        MainPanel.SetActive(false);
        SavesPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        GameDataManager.LoadLastGameSave();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadGame(GameSaveInfo gameSave)
    {
        MainPanel.SetActive(false);
        SavesPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        GameDataManager.LoadGameSave(gameSave);
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowSaves()
    {
        if(_saves == null)
        {
            _saves = CRUD.GetAllGameSaves(GameData.CurrentUser);
            foreach(var save in _saves)
            {
                GameObject saveObject = Instantiate(SaveSlotPrefab, SavesTransformParent);
                saveObject.GetComponent<SaveVisualizer>().Setup(save);
            }
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
