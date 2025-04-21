using CRPG;
using CRPG.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private InventoryPanel _inventoryPanel;
    [SerializeField] private EquipmentPanel _equipmentPanel;
    public GameObject PauseMenuPanel;
    public Button SaveButton;
    [SerializeField] private PlayerPanel _playerPanel;
    private PlayerPanelViewModel _playerPanelViewModel;

	public RectTransform PointerInfoPanel;
    public BattlePanel BattlePanel;
    public GameObject DeathPanel;

    public bool IsInventoryOpen => _inventoryPanel.IsOpen;
    public bool IsPauseMenuOpen { get; private set; } = false;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private GraphicRaycaster _graphicRaycaster;
    private readonly List<RaycastResult> _raycastResultsList = new List<RaycastResult>();
    [SerializeField] private Text _pointerInfoText;

    public UnityEvent<Item> OnDropItem => _inventoryPanel.OnDropItem;

	private void OnValidate()
	{
		if(_graphicRaycaster == null) _graphicRaycaster = GetComponent<GraphicRaycaster>();
        if(_pointerInfoText == null) _pointerInfoText = PointerInfoPanel.GetComponentInChildren<Text>();
        if(_canvas == null) _canvas = gameObject.GetComponent<Canvas>();
	}

    internal void Setup(UnityEvent DeathEvent, Func<ActionType,PersonageActionInfo> GetActionInfo)
    {
        DeathEvent.AddListener(OnDeath);
		_playerPanelViewModel = new PlayerPanelViewModel(_playerPanel, GetActionInfo);
	}

	private void Update()
    {
		if (IsInventoryOpen && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            _raycastResultsList.Clear();
            _graphicRaycaster.Raycast(pointer, _raycastResultsList);
            _inventoryPanel.ProcessRaycast(_raycastResultsList);
        }
        if (PointerInfoPanel.gameObject.activeInHierarchy)
        {
			PointerInfoPanel.position = Input.mousePosition;
		}
    }

    public void ToggleInventory(List<Item> inventory, EquipmentManager equipmentManager)
    {
        _inventoryPanel.ToggleInventory(inventory);
        _equipmentPanel.Setup(equipmentManager);
    }

    public void TogglePauseMenu()
    {
        IsPauseMenuOpen = !IsPauseMenuOpen;
        PauseMenuPanel.SetActive(IsPauseMenuOpen);
        SaveButton.interactable = GameManager.Instance.GameMode == GameMode.Free;
    }

    public void SetActivePersonage(PlayerController personageController, EquipmentManager equipmentManager)
    {
        _playerPanelViewModel.SetActivePersonage(personageController);
        _inventoryPanel.SetActivePlayer(equipmentManager);
    }

    public void OnChangeGameMode(GameMode lastGameMode, GameMode currentGameMode)
    {
		_playerPanelViewModel.OnChangeGameMode(lastGameMode, currentGameMode);
        if(currentGameMode == GameMode.Dialogue)
        {
            HideInfoUnderPointer();
        }
    }

    public void ShowInfoUnderPosition(string info, Vector3 position)
    {
        PointerInfoPanel.position = Input.mousePosition;
        PointerInfoPanel.gameObject.SetActive(true);
        _pointerInfoText.text = info;
    }
    
    public void HideInfoUnderPointer()
    {
        if(PointerInfoPanel != null)
        {
			PointerInfoPanel.gameObject.SetActive(false);
		}
    }

    public void OnDeath()
    {
        DeathPanel.SetActive(true);
    }

	private void OnDestroy()
	{
        _playerPanelViewModel.Dispose();
	}
}