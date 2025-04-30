using CRPG.ItemSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CRPG.UI
{
    class ProjectileSlotUI : ItemSlotUI
	{
		[SerializeField] private Sprite defaultSprite;
		[SerializeField] private TextMeshPro _text;
		public override ItemSlot ItemSlot => _projectileSlot;

		private ProjectileSlot _projectileSlot;

		private void OnValidate()
		{
			if (_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		public void Setup(ProjectileSlot projectileSlot)
		{
			ReleaseSlot();
			_projectileSlot = projectileSlot;
			if (_projectileSlot.ProjectileItems != null)
			{
				EquipItem(_projectileSlot.ProjectileItemInfo);
			}
			_projectileSlot.OnEquipItems += EquipItem;
			_projectileSlot.OnUnequipItems += UnequipItem;
		}

		public void ReleaseSlot()
		{
			if (_projectileSlot != null)
			{
				_projectileSlot.OnEquipItems -= EquipItem;
				_projectileSlot.OnUnequipItems -= UnequipItem;
				UnequipItem();
			}
		}

		private void OnDestroy()
		{
			ReleaseSlot();
		}

		private void EquipItem(ProjectileItemInfo itemInfo)
		{
			base.EquipItem(itemInfo);
			_text.text = _projectileSlot.ProjectileItems.Count.ToString();
		}

		private void UnequipItem(List<ProjectileItem> list)
		{
			UnequipItem();
		}

		private void UnequipItem()
		{
			_iconImage.sprite = defaultSprite;
		}
	}
}