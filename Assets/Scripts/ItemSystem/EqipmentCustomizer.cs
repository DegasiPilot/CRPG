using UnityEngine;

public class EqipmentCustomizer : MonoBehaviour
{
    public static EqipmentCustomizer Instance;

    public SkinPart Helmet;
    public SkinPart Body;
    public SkinPart Boots;
    public SkinPart RigthHand;
    public SkinPart LeftHand;


    public void Awake()
    {
        Instance = this;
    }

    public void EquipSkin(GameObject skin, BodyPart bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPart.LeftHand:
                LeftHand.SetSkin(skin);
                break;
            case BodyPart.RightHand:
                RigthHand.SetSkin(skin);
                break;
            case BodyPart.Head:
                Helmet.SetSkin(skin);
                if (Body.IsEmpty) Body.ResetSkin(true);
                break;
            case BodyPart.Body:
                Body.SetSkin(skin);
                break;
            case BodyPart.Legs:
                Boots.SetSkin(skin);
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
                if (Body.IsEmpty) Body.ResetSkin(false);
                break;
            case BodyPart.Body:
                Body.ResetSkin(!Helmet.IsEmpty);
                break;
            case BodyPart.Legs:
                Boots.ResetSkin();
                break;
        }
    }
}
