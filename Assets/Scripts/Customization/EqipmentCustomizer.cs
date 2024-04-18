using UnityEngine;

public class EqipmentCustomizer : MonoBehaviour
{
    public static EqipmentCustomizer Instance;

    public ArmorSkinPart Helmet;
    public ArmorSkinPart Body;
    public ArmorSkinPart Boots;
    public ItemSkin RigthHand;
    public ItemSkin LeftHand;


    public void Awake()
    {
        Instance = this;
    }


    public void EquipSkin(Item item, BodyPart bodyPart)
    {
        if(bodyPart == BodyPart.LeftHand || bodyPart == BodyPart.RightHand)
        {
            EquipItemSkin(item.gameObject, bodyPart);
        }
        else
        {
            EquipArmorSkin((item.ItemInfo as ArmorInfo).SkinIndex, bodyPart);
        }
    }

    public void EquipArmorSkin(int index, BodyPart bodyPart)
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

    public void EquipItemSkin(GameObject skin, BodyPart bodyPart)
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

    public void ResetSkin(BodyPart bodyPart)
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
}
