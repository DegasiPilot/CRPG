using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInfo", menuName = "ScriptableObjects/ПредметИнфо")]
public class ItemInfo : ScriptableObject
{
	public string Name;
	[TextArea] public string ShortDescription;
	public Sprite Icon;
	public bool IsStackable;

	public virtual string GetFullDescrition()
	{
		return ShortDescription;
	}
}