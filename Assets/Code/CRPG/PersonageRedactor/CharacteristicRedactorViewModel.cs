namespace CRPG.PersonageRedactor
{
    class CharacteristicRedactorViewModel
    {
		private PersonageCreatorViewModel _creator;
		private CharacteristicRedactor _view;

		public CharacteristicRedactorViewModel(PersonageCreatorViewModel creator, CharacteristicRedactor view)
		{
			_view = view;
			_creator = creator;
			_view.Setup(AddPoint, RemovePoint, CanAddMore, CanRemoveMore, GetAmount);
			_creator.OnNoMoreStatPoints += DeactivatePlusButton;
			_creator.OnGetStatPoints += TryActivatePlusButton;
		}

		private void DeactivatePlusButton() => _view.DeactivatePlusButton();
		private void TryActivatePlusButton() => _view.TryActivatePlusButton();

		private void AddPoint(Characteristics characteristic)
		{
			_creator.AddStatPoint(characteristic);
		}

		private void RemovePoint(Characteristics characteristic)
		{
			_creator.RemoveStatPoint(characteristic);
		}

		private bool CanAddMore(Characteristics characteristic)
		{
			return _creator.CanAddMore(characteristic);
		}

		private bool CanRemoveMore(Characteristics characteristic)
		{
			return _creator.CanRemoveMore(characteristic);
		}

		public void UpdateAmount()
		{
			_view.UpdateAmount(_creator.GetStatValue(_view.Characteristic));
		}

		public int GetAmount(Characteristics characteristic)
		{
			var statValue = _creator.GetStatValue(characteristic);
			return statValue.Item1 + statValue.Item2;
		}
	}
}
