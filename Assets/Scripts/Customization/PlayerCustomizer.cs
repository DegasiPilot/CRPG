using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    public Material PlayerMaterial;
    public GameObject MaleObject;
    public GameObject FemaleObject;
    public List<GameObject> MaleHairs;
    public List<GameObject> FemaleHairs;
    public List<GameObject> MaleFaces;
    public List<GameObject> FemaleFaces;
    public List<GameObject> Beards;

    [System.NonSerialized] public List<GameObject> Hairs;
    [System.NonSerialized] public List<GameObject> Faces;

    public int _hairColorPropertyID;
    public int _skinColorPropertyID;
    private PersonageInfo.AppearanceStruct _appearance => GameData.PlayerPersonageInfo.Appearance;
    private Gender _gender => GameData.PlayerPersonageInfo.Gender;

    private void Awake()
    {
        _hairColorPropertyID = Shader.PropertyToID("_HAIRCOLOR");
        _skinColorPropertyID = Shader.PropertyToID("_SKINCOLOR");
    }

    public Color GetHairsColor()
    {
        return PlayerMaterial.GetColor(_hairColorPropertyID);
    }

    public Color GetSkinColor()
    {
        return PlayerMaterial.GetColor(_skinColorPropertyID);
    }

    public void ApplyAppearance()
    {
        ChangeHairColor(_appearance.HairsColor);
        ChangeSkinColor(_appearance.SkinColor);
        ChangeGender(_gender);
        Faces.ForEach(x => x.SetActive(false));
        Faces[_appearance.FaceIndex].SetActive(true);
        ApplyPart(_appearance.HairIndex, Hairs);
        if (_gender == Gender.Male) ApplyPart(_appearance.BeardIndex, Beards);
    }

    public void ChangeHairColor(Color color)
    {
        PlayerMaterial.SetColor(_hairColorPropertyID, color);
    }

    public void ChangeSkinColor(Color color)
    {
        PlayerMaterial.SetColor(_skinColorPropertyID, color);
    }

    public void ChangeGender(Gender gender)
    {
        if (gender == Gender.Male)
        {
            MaleObject.SetActive(true);
            FemaleObject.SetActive(false);
            Hairs = MaleHairs;
            Faces = MaleFaces;
        }
        else
        {
            MaleObject.SetActive(false);
            FemaleObject.SetActive(true);
            Hairs = FemaleHairs;
            Faces = FemaleFaces;
        }
    }

    private void ApplyPart(int _activeIndex, List<GameObject> _parts)
    {
        if (_activeIndex == 0)
        {
            _parts.ForEach(x => x.SetActive(false));
        }
        else if (_activeIndex == _parts.Count + 1)
        {
            _parts.ForEach(x => x.SetActive(true));
        }
        else
        {
            _parts.ForEach(x => x.SetActive(false));
            _parts[_activeIndex - 1].SetActive(true);
        }
    }
}