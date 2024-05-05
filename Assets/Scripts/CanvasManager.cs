using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject InventoryPanel;
    public ItemContextMenu ItemContextMenu;
    public ItemInfoPanel ItemInfoPanel; 
    public GameObject PauseMenuPanel;
    public Button SaveButton;
    public GameObject PlayerPanel;
    public RawImage PersonageImage;

    public Text PersonageNameText;
    public Text LifesText;
    public Image LifesImage;
    public ToggleGroup ActionsToggles;
    public GameObject PointerInfoPanel;
    public BattlePanel BattlePanel;

    public bool IsInventoryOpen { get; private set; } = false;
    public bool IsPauseMenuOpen { get; private set; } = false;

    private Canvas _canvas;
    private List<InventorySlot> _inventorySlots;
    private ItemSlot _activeItemSlot;
    private GraphicRaycaster _graphicRaycaster;
    private readonly List<RaycastResult> _raycastResultsList = new List<RaycastResult>();
    private Text _pointerInfoText;

    private PlayerController _playerController => GameManager.Instance.PlayerController;

    public void Awake()
    {
        Instance = this;
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
        _inventorySlots = InventoryPanel.GetComponentsInChildren<InventorySlot>(true).ToList();
        _pointerInfoText = PointerInfoPanel.GetComponentInChildren<Text>();
        _canvas = gameObject.GetComponent<Canvas>();
    }

    private void Start()
    {
        GameManager.Instance.OnHealthChanged.AddListener(UpdateHealtBar);
    }

    private void UpdateHealtBar()
    {
        LifesImage.fillAmount = (float)GameManager.Instance.PlayerPersonage.CurrentHealth / GameManager.Instance.PlayerPersonage.MaxHealth;
        Personage player = GameManager.Instance.PlayerPersonage;
        LifesText.text = $"{player.CurrentHealth}/{player.MaxHealth}";
    }

    private void Update()
    {
        if (InventoryPanel.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            _raycastResultsList.Clear();
            _graphicRaycaster.Raycast(pointer, _raycastResultsList);
            if (_raycastResultsList.Any())
            {
                if (_raycastResultsList[0].gameObject.TryGetComponent(out ItemSlot itemSlot))
                {
                    if (itemSlot.Item != null)
                    {
                        ActivateContextMenu(itemSlot);
                        return;
                    }
                }
                else if(_raycastResultsList[0].gameObject.TryGetComponent(out Button button))
                {
                    if (button == ItemContextMenu.EquipmentButton)
                    {
                        if (button.interactable)
                        {
                            OnEquipButtonClick();
                        }
                    }
                    else if (button == ItemContextMenu.InfoButton)
                    {
                        if (button.interactable)
                        {
                            OnInfoButtonClick();
                        }
                    }
                    else if (button == ItemContextMenu.DropButton)
                    {
                        if (button.interactable)
                        {
                            OnDropButtonClick();
                        }
                    }
                    else if(button == ItemInfoPanel.CloseButton)
                    {
                        ItemInfoPanel.Close();
                    }
                }
            }
            ItemContextMenu.gameObject.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (!IsInventoryOpen)
        {
            InventoryPanel.SetActive(true);
            PlayerPanel.SetActive(false);
            int inventoryCount = GameData.Inventory.Count;
            for (int i = 0; i < inventoryCount; i++)
            {
                if (GameData.Inventory[i].IsEquiped)
                {
                    continue;
                }
                _inventorySlots[i].gameObject.SetActive(true);
                _inventorySlots[i].Setup(GameData.Inventory[i]);
            }
            for(int i = inventoryCount; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].gameObject.SetActive(false);
            }
        }
        else
        {
            InventoryPanel.SetActive(false);
            PlayerPanel.SetActive(true);
        }
        IsInventoryOpen = !IsInventoryOpen;
    }

    public void TogglePauseMenu()
    {
        IsPauseMenuOpen = !IsPauseMenuOpen;
        PauseMenuPanel.SetActive(IsPauseMenuOpen);
        SaveButton.interactable = GameManager.Instance.GameMode == GameMode.Free;
    }

    public void ActivateContextMenu(ItemSlot itemSlot)
    {
        ItemContextMenu.transform.position = itemSlot.transform.position;
        ItemContextMenu.gameObject.SetActive(true);
        ItemContextMenu.Setup(itemSlot.Item);
        _activeItemSlot = itemSlot;
    }

    public void OnEquipButtonClick()
    {
        if (!_activeItemSlot.Item.IsEquiped)
        {
            EquipmentManager.Instance.EquipItem(_activeItemSlot.Item, out List<Item> undressedItems);
            undressedItems.ForEach(AddToInventoryUI);
            _activeItemSlot.UnequipItem();
        }
        else
        {
            EquipmentManager.Instance.UneqipItemFromSlot((EquipmentSlot)_activeItemSlot, out Item undressedItem);
            AddToInventoryUI(undressedItem);
        }
        _activeItemSlot = null;
    }

    public void OnInfoButtonClick()
    {
        ItemInfoPanel.gameObject.SetActive(true);
        ItemInfoPanel.Setup(_activeItemSlot.Item.ItemInfo);
        _activeItemSlot = null;
    }

    public void OnDropButtonClick()
    {
        GameManager.Instance.PlayerController.DropItem(_activeItemSlot.Item);
        _activeItemSlot.gameObject.SetActive(false);
        ItemInfoPanel.Close();
        _activeItemSlot = null;
    }

    private void AddToInventoryUI(Item item)
    {
        foreach(var slot in _inventorySlots)
        {
            if(slot.Item == null)
            {
                slot.gameObject.SetActive(true);
                slot.Setup(item);
                break;
            }
        }
    }

    public void SetActivePersonage(Personage personage)
    {
        PersonageNameText.text = personage.PersonageInfo.Name;
        int MaxHealth = personage.PersonageInfo.MaxHealth;
        LifesText.text = $"Жизни: {personage.CurrentHealth}/{MaxHealth}";
        LifesImage.fillAmount = personage.CurrentHealth / MaxHealth;
        PersonageImage.texture = personage.PersonageInfo.PersonagePortrait;
    }

    public void TogglePlayerAction(ActionType actionType, bool activate)
    {
        if (activate)
        {
            _playerController.SetActiveAction(actionType);
        }
        else
        {
            _playerController.SetDefaultAction();
        }
    }

    public void ForceChangeAction(ActionType actionType)
    {
        if(actionType == ActionType.Movement)
        {
            ActionsToggles.SetAllTogglesOff();
        }
    }

    public void OnChangeGameMode(GameMode lastGameMode, GameMode currentGameMode)
    {
        if(lastGameMode == GameMode.Dialogue)
        {
            PlayerPanel.SetActive(true);
            HideInfoUnderPointer();
        }
        else if(currentGameMode == GameMode.Dialogue)
        {
            PlayerPanel.SetActive(false);
        }
    }

    public void ShowInfoUnderPosition(string info, Vector3 position)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((_canvas.transform as RectTransform), screenPoint, Camera.main, out Vector2 localPoint);
        (PointerInfoPanel.transform as RectTransform).anchoredPosition = localPoint;
        PointerInfoPanel.SetActive(true);
        _pointerInfoText.text = info;
    }
    
    public void HideInfoUnderPointer()
    {
        PointerInfoPanel.SetActive(false);
    }
}