using System;
using System.Collections;

namespace CRPG.Battle
{
    class AttackModule : IDisposable
    {
        public AttackModule(PersonageController personageController, ChooseAttackForceModule chooseAttackForceModule)
        {
			_personageController = personageController;
            _chooser = chooseAttackForceModule;
			_chooser.OnChooseAttackForce.AddListener(Attack);
		}

		public bool IsAttacking { get; private set; }

        private ChooseAttackForceModule _chooser;
		private PersonageController _personageController;
		private PersonageController _enemy;

		public void Attack(PersonageController enemy)
		{
			_enemy = enemy;
			IsAttacking = true;
			_chooser.ChooseAttackForce(_personageController.Personage);
		}

		private void Attack(float energy)
		{
			_personageController.MakeDamage(energy, _enemy);
			_personageController.Personage.RemoveStamina(energy);
			BattleManager.AfterAttack(_personageController);
			if(IsAttacking) _chooser.ChooseAttackForce(_personageController.Personage);
		}

		public void EndAttack()
		{
			IsAttacking = false;
		}

		public void Dispose()
		{
			_chooser.OnChooseAttackForce.RemoveListener(Attack);
		}
	}
}