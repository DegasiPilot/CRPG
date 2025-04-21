using System.Collections.Generic;
using CRPG.Customization;
using CRPG.DataSaveSystem;
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
	[System.NonSerialized] public EquipmentCustomizer EquipmentCustomizer;

    [System.NonSerialized] public List<GameObject> Hairs;
    [System.NonSerialized] public List<GameObject> Faces;

	[SerializeField] private int _hairColorPropertyID;
	[SerializeField] private int _skinColorPropertyID;
    private Gender _gender => _player.PersonageInfo.Gender;

    private Personage _player;
    private AppearanceStruct _appearanceStruct;
    internal AppearanceStruct AppearanceStruct => _appearanceStruct;

    public void Setup(Personage player)
    {
        _player = player;
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

    internal void ApplyAppearance(AppearanceStruct appearance)
    {
        _appearanceStruct = appearance;
        ChangeHairColor(appearance.HairsColor);
        ChangeSkinColor(appearance.SkinColor);
        ChangeGender(_gender);
        Faces.ForEach(x => x.SetActive(false));
        Faces[appearance.FaceIndex].SetActive(true);
        ApplyPart(appearance.HairIndex, Hairs);
        if (_gender == Gender.Male) 
        {
            Destroy(FemaleObject);
            ApplyPart(appearance.BeardIndex, Beards);
            EquipmentCustomizer = MaleObject.GetComponent<EquipmentCustomizer>();
        }
        else
        {
            Destroy(MaleObject);
            EquipmentCustomizer = FemaleObject.GetComponent<EquipmentCustomizer>();
        }
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
            var a = GameData.MainPlayer;
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