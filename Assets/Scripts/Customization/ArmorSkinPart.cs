using System.Collections.Generic;
using UnityEngine;

public class ArmorSkinPart : MonoBehaviour
{
    public GameObject[] SkinVariants;
    public int ActiveIndex { get; private set; }

    public void SetSkin(int index)
    {
        SkinVariants[index].SetActive(true);
        ActiveIndex = index;
    }

    public void ResetSkin()
    {
        SkinVariants[ActiveIndex].SetActive(false);
        SkinVariants[0].SetActive(true);
        ActiveIndex = 0;
    }
}
