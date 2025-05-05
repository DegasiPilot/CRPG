using CRPG.ItemSystem;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCustomizer : MonoBehaviour
{
    public ArmorSkinPart Helmet;
    public ArmorSkinPart Body;
    public ArmorSkinPart Boots;
    public ArmorSkinPart ArrowsPart;
    public ItemSkin RigthHand;
    public ItemSkin LeftHand;

    [SerializeField] private EquipmentManager _equipmentManager;

	private void Awake()
	{
        if (_equipmentManager == null) Debug.LogError("there!", this);
        _equipmentManager.OnItemEquiped += EquipSkin;
        _equipmentManager.OnItemUnequiped += ResetSkin;
        _equipmentManager.OnProjectileEquiped += EquipArrows;
        _equipmentManager.OnProjectileUnequiped += ResetArrows;
	}

	private void EquipSkin(EquipableItem item, BodyPart bodyPart)
    {
        if(bodyPart == BodyPart.LeftHand || bodyPart == BodyPart.RightHand)
        {
            EquipItemSkin(item, bodyPart);
        }
        else if(item is Armor armor)
        {
            EquipArmorSkin(armor.ArmorInfo.SkinIndex, bodyPart);
        }
    }

    private void EquipArrows()
    {
        ArrowsPart.SetSkin(1);
    }

	private void ResetArrows(List<ProjectileItem> list)
	{
        ResetArrows();
	}

	private void ResetArrows()
	{
		ArrowsPart.ResetSkin();
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

    private void EquipItemSkin(EquipableItem item, BodyPart bodyPart)
    {
        if(item is Weapon weapon && weapon.WeaponInfo.IsTwoHandled)
        {
            if(bodyPart == BodyPart.LeftHand)
            {
				LeftHand.SetSkin(item);
			}
		}
        else
        {
			switch (bodyPart)
			{
				case BodyPart.LeftHand:
					LeftHand.SetSkin(item);
					break;
				case BodyPart.RightHand:
					RigthHand.SetSkin(item);
					break;
			}
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
            case BodyPart.Arrows:
                ResetArrows();
                break;
        }
    }

    private void Release()
    {
		_equipmentManager.OnItemEquiped -= EquipSkin;
		_equipmentManager.OnItemUnequiped -= ResetSkin;
        _equipmentManager.OnProjectileEquiped -= EquipArrows;
		_equipmentManager.OnProjectileUnequiped -= ResetArrows;
	}

	private void OnDestroy()
	{
        Release();
	}
}
