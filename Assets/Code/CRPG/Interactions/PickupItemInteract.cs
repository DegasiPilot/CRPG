

namespace CRPG.Interactions
{
    class PickupItemInteract : Interact
    {
		private Item _item;

		public PickupItemInteract(Item item)
		{
			_item = item;
		}

		public override void Execute(PersonageController executor)
		{
			executor.PickupItem(_item);
		}
	}
}