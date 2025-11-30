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
		[SerializeField] private TextMeshProUGUI _text;
		public override ItemSlot ItemSlot => _projectileSlot;

		private ProjectileSlot _projectileSlot;

		protected override void OnValidate()
		{
			base.OnValidate();
			if (_iconImage == null) _iconImage = transform.GetChild(0).GetComponent<Image>();
		}

		public void Setup(ProjectileSlot projectileSlot)
		{
			ReleaseSlot();
			_projectileSlot = projectileSlot;
			if (_projectileSlot.ProjectileItems != null)
			{
				EquipProjectile();
			}
			_projectileSlot.OnEquipProjectile += EquipProjectile;
			_projectileSlot.OnUnequipItems += UnequipItem;
		}

		internal void ReleaseSlot()
		{
			if (_projectileSlot != null)
			{
				_projectileSlot.OnEquipProjectile -= EquipProjectile;
				_projectileSlot.OnUnequipItems -= UnequipItem;
				UnequipItem();
				_projectileSlot = null;
			}
		}

		private void OnDestroy()
		{
			ReleaseSlot();
		}

		private void EquipProjectile()
		{
			base.EquipItem(_projectileSlot.ProjectileItems[0].ProjectileItemInfo);
			_text.text = _projectileSlot.ProjectileItems.Count.ToString();
		}

		private void UnequipItem(List<ProjectileItem> list)
		{
			UnequipItem();
		}

		protected override void UnequipItem()
		{
			base.UnequipItem();
			_iconImage.sprite = defaultSprite;
			_text.text = string.Empty;
		}
	}
}