using System;
using System.Collections.Generic;

namespace CRPG.ItemSystem
{
	class ProjectileSlot : ItemSlot
	{
		public List<ProjectileItem> ProjectileItems { get; private set; }
		public ProjectileItemInfo ProjectileItemInfo => ProjectileItems?[0].ProjectileItemInfo;
		public event Action OnEquipProjectile;
		public event Action<List<ProjectileItem>> OnUnequipItems;

		public override void ClearSlot()
		{
			foreach (var item in ProjectileItems)
			{
				item.IsEquiped = false;
			}
			OnUnequipItems.Invoke(ProjectileItems);
			ProjectileItems = null;
		}

		public override void OnDropButtonClick(Action<Item> dropItemCallback)
		{
			foreach (ProjectileItem projectile in ProjectileItems)
			{
				dropItemCallback.Invoke(projectile);
			}
			ClearSlot();
		}

		public override bool OnEquipButtonClick(EquipmentManager equipmentManager)
		{
			if (ProjectileItems != null)
			{
				equipmentManager.UnequipProjectile(this);
				return false;
			}
			else
			{
				return false;
			}
		}

		public override void SetupItemInfoPanel(ItemInfoPanel itemInfoPanel)
		{
			itemInfoPanel.Setup(ProjectileItems[0].ItemInfo);
		}

		public override bool TrySetupItemContextMenu(ItemContextMenu itemContextMenu)
		{
			if (ProjectileItems != null)
			{
				itemContextMenu.Setup(ProjectileItems[0].ItemInfo, true, true);
				return true;
			}
			else
			{
				return false;
			}
		}

		internal void EquipProjectile(ProjectileItem projectileItem)
		{
			projectileItem.IsEquiped = true;
			if (ProjectileItems == null)
			{
				ProjectileItems = new List<ProjectileItem>() { projectileItem };
			}
			else
			{
				ProjectileItems.Add(projectileItem);
			}
			OnEquipProjectile.Invoke();
		}

		internal void EquipProjectiles(IEnumerable<ProjectileItem> projectileItems)
		{
			if (ProjectileItems == null)
			{
				ProjectileItems = new List<ProjectileItem>(projectileItems);
			}
			else
			{
				ProjectileItems.AddRange(projectileItems);
			}
			foreach (var item in ProjectileItems)
			{
				item.IsEquiped = true;
			}
			OnEquipProjectile.Invoke();
		}

		internal ProjectileItem GetOne()
		{
			int count = ProjectileItems.Count;
			int lastIndex = ProjectileItems.Count - 1;
			ProjectileItem projectileItem = ProjectileItems[lastIndex];
			if (count == 1)
			{
				ProjectileItems = null;
			}
			else
			{
				ProjectileItems.RemoveAt(lastIndex);
			}
			projectileItem.IsEquiped = false;
			return projectileItem;
		}
	}
}