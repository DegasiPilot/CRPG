using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueParser dialogueParser;
    public GameObject Player;
    public Personage SecondPersonage;
    public CameraController CameraController;

    public PlayerController PlayerController;
    
    private GameMode _gameMode;
    private Personage _playerPersonage;

    void Start()
    {
        Instance = this;
        dialogueParser.Setup();
        _playerPersonage = Player.GetComponent<Personage>();
        dialogueParser.PlayerPersonage = _playerPersonage;
        dialogueParser.SecondPersonage = SecondPersonage;
        PlayerController = _playerPersonage.GetComponent<PlayerController>();
        PlayerController.Setup();
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        _gameMode = gameMode;
        switch (gameMode)
        {
            case GameMode.Dialogue:
                CameraController.enabled = false;
                break;
            case GameMode.Free:
                CameraController.enabled = true;
                break;
        }
    }
}
