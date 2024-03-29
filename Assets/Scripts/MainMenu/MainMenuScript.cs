using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public static MainMenuScript Instance;

    public Transform SavesTransformParent;
    public GameObject SaveSlotPrefab;
    private List<GameSaveInfo> _saves;

    private void Awake()
    {
        Instance = this;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("PersonageRedactorScene");
    }

    public void LoadLastGame()
    {
        GameData.LoadLastGameSave();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadGame(GameSaveInfo gameSave)
    {
        GameData.LoadGameSave(gameSave);
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
