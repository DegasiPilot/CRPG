using UnityEngine;

public class ItemSkin : MonoBehaviour
{
    private GameObject _activeSkin;

    public void SetSkin(GameObject skin)
    {
        _activeSkin = skin;
        skin.transform.SetParent(transform);
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localRotation = Quaternion.identity;
        _activeSkin.SetActive(true);
    }

    public void ResetSkin()
    {
        _activeSkin.transform.parent = null;
        _activeSkin.SetActive(false);
        _activeSkin = null;
    }
}