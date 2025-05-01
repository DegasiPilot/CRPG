using CRPG.ItemSystem;
using UnityEngine;

public class ItemSkin : MonoBehaviour
{
    private EquipableItem _activeSkin;
    public EquipableItem ActiveSkin => _activeSkin;

    public void SetSkin(EquipableItem skin)
    {
        _activeSkin = skin;
        skin.transform.SetParent(transform);
        skin.transform.localPosition = Vector3.zero;
		skin.transform.localRotation = Quaternion.identity;
		if (skin.RotationInHand != Vector3.zero)
        {
            skin.transform.Rotate(skin.RotationInHand);
		}
        _activeSkin.gameObject.SetActive(true);
    }

    public void ResetSkin()
    {
        _activeSkin.transform.parent = null;
		_activeSkin.gameObject.SetActive(false);
		_activeSkin = null;
    }
}