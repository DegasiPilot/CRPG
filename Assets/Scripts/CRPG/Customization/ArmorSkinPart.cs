using System.Collections.Generic;
using UnityEngine;

public class ArmorSkinPart : MonoBehaviour
{
    public GameObject[] SkinVariants;
    public int ActiveIndex { get; private set; }

    public void SetSkin(int index)
    {
        if (SkinVariants[0] != null)
        {
            SkinVariants[0].SetActive(false);
        }
        SkinVariants[index].SetActive(true);
        ActiveIndex = index;
    }

    public void ResetSkin()
    {
        if (SkinVariants[0] != null)
        {
            SkinVariants[0].SetActive(true);
        }
        SkinVariants[ActiveIndex].SetActive(false);
        ActiveIndex = 0;
    }
}
