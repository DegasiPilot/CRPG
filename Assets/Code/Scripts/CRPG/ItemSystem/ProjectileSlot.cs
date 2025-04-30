using System;
using System.Collections.Generic;

namespace CRPG.ItemSystem
{
	class ProjectileSlot : ItemSlot
	{
		public List<ProjectileItem> ProjectileItems { get; private set; }
		public ProjectileItemInfo ProjectileItemInfo => ProjectileItems?[0].ProjectileItemInfo;
		public event Action<ProjectileItemInfo> OnEquipItems;
		public event Action<List<ProjectileItem>> OnUnequipItems;

		public override void ClearSlot()
		{
			OnUnequipItems.Invoke(ProjectileItems);
			ProjectileItems = null;
		}

		public override void OnDropButtonClick(Action<Item> dropItemCallback)
		{
			foreach(ProjectileItem projectile in ProjectileItems)
			{
				dropItemCallback.Invoke(projectile);
			}
			ClearSlot();
		}

		public override bool OnEquipButtonClick(EquipmentManager equipmentManager)
		{
			if(ProjectileItems != null)
			{
				equipmentManager.UnequipProjectile(this);
				return false;
			}
			else
			{
				return false;
			}
		}

		public override void SetupItemItemInfoPanel(ItemInfoPanel itemInfoPanel)
		{
			itemInfoPanel.Setup(ProjectileItems[0].ItemInfo);
		}

		public override bool TrySetupItemContextMenu(ItemContextMenu itemContextMenu)
		{
			if (ProjectileItems[0] != null)
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
			if(ProjectileItems == null)
			{
				ProjectileItems = new List<ProjectileItem>() { projectileItem };
				OnEquipItems.Invoke(projectileItem.ProjectileItemInfo);
			}
			else
			{
				ProjectileItems.Add(projectileItem);
			}
		}

		internal ProjectileItem GetOne()
		{
			int lastIndex = ProjectileItems.Count - 1;
			ProjectileItem projectileItem = ProjectileItems[lastIndex];
			ProjectileItems.RemoveAt(lastIndex);
			projectileItem.IsEquiped = false;
			return projectileItem;
		}
	}
}