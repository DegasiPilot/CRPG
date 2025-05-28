using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorInfo", menuName = "ScriptableObjects/БроняInfo")]
internal class ArmorInfo : ItemInfo
{
	public int SkinIndex;
	[SerializeField, Range(0, 100)] private int _armorPercent;
	public float ArmorPercent => _armorPercent / 100f;
	public BodyPart WearableBodyPart;
	[Min(0)] public float ArmorWeight;
	[Range(0, 1)] public float DodgePenalty;

	public override string GetFullDescrition()
	{
		return base.GetFullDescrition() + '\n' + "Коэффициент брони: " + _armorPercent + '%';
	}
}