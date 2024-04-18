using UnityEngine;

public class ItemSkin : MonoBehaviour
{
    private GameObject _activeSkin;

    public void SetSkin(GameObject skin)
    {
        _activeSkin = skin;
        skin.transform.SetParent(transform);
        _activeSkin.SetActive(true);
    }

    public void ResetSkin()
    {
        _activeSkin.transform.parent = null;
        _activeSkin.SetActive(false);
        _activeSkin = null;
    }
}