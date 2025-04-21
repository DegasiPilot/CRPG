using CRPG.ItemSystem;
using UnityEngine;

public class EquipmentCustomizer : MonoBehaviour
{
    public ArmorSkinPart Helmet;
    public ArmorSkinPart Body;
    public ArmorSkinPart Boots;
    public ItemSkin RigthHand;
    public ItemSkin LeftHand;

    [SerializeField] private EquipmentManager _equipmentManager;

	private void Awake()
	{
        if (_equipmentManager == null) Debug.LogError("there!", this);
        _equipmentManager.OnItemEquiped += EquipSkin;
        _equipmentManager.OnItemUnequiped += ResetSkin;
	}

	private void EquipSkin(Item item, BodyPart bodyPart)
    {
        if(bodyPart == BodyPart.LeftHand || bodyPart == BodyPart.RightHand)
        {
            EquipItemSkin(item.gameObject, bodyPart);
        }
        else if(item is Armor armor)
        {
            EquipArmorSkin(armor.ArmorInfo.SkinIndex, bodyPart);
        }
    }

    private void EquipArmorSkin(int index, BodyPart bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                Helmet.SetSkin(index);
                break;
            case BodyPart.Body:
                Body.SetSkin(index);
                break;
            case BodyPart.Legs:
                Boots.SetSkin(index);
                break;
        }
    }

    private void EquipItemSkin(GameObject skin, BodyPart bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPart.LeftHand:
                LeftHand.SetSkin(skin);
                break;
            case BodyPart.RightHand:
                RigthHand.SetSkin(skin);
                break;
        }
    }

	private void ResetSkin(Item item, BodyPart bodyPart)
	{
        ResetSkin(bodyPart);
	}

	private void ResetSkin(BodyPart bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPart.LeftHand:
                LeftHand.ResetSkin();
                break;
            case BodyPart.RightHand:
                RigthHand.ResetSkin();
                break;
            case BodyPart.Head:
                Helmet.ResetSkin();
                break;
            case BodyPart.Body:
                Body.ResetSkin();
                break;
            case BodyPart.Legs:
                Boots.ResetSkin();
                break;
        }
    }

    private void Release()
    {
		_equipmentManager.OnItemEquiped -= EquipSkin;
		_equipmentManager.OnItemUnequiped -= ResetSkin;
	}

	private void OnDestroy()
	{
        Release();
	}
}
