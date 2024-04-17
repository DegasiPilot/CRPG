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

    private List<GameSaveInfo> _saves;

    private void Awake()
    {
        Instance = this;
        bool hasSaves = CRUD.HasAnySaves();
        ContinueBtn.interactable = hasSaves;
        LoadBtn.interactable = hasSaves;
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
            _saves = CRUD.GetAllGameSaves();
            foreach(var save in _saves)
            {
                GameObject saveObject = Instantiate(SaveSlotPrefab, SavesTransformParent);
                saveObject.GetComponent<SaveVisualizer>().Setup(save);
            }
        }
    }
}
