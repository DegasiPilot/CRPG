

namespace CRPG.Interactions
{
	class AttackInteract : Interact
	{
		PersonageController _enemy;

		public AttackInteract(PersonageController enemy)
		{
			_enemy = enemy;
		}

		public override void Execute(PersonageController executor)
		{
			executor.StartAttack(_enemy);
		}
	}
}