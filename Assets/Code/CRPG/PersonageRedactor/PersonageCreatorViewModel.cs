using CRPG.Customization;
using CRPG.DataSaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CRPG.PersonageRedactor
{
	class PersonageCreatorViewModel : IDisposable
	{
		public event Action OnNoMoreStatPoints;
		public event Action OnGetStatPoints;

		public List<GameObject> Hairs => _playerCustomizer.Hairs;
		public List<GameObject> Faces => _playerCustomizer.Faces;
		public List<GameObject> Beards => _playerCustomizer.Beards;

		private GameObject MaleObject => _playerCustomizer.MaleObject;
		private GameObject FemaleObject => _playerCustomizer.FemaleObject;

		private PersonageInfo _personageInfo;
		private const int _minPointsForStat = 0;
		private const int _maxPointsForStat = 9;

		private int _maxStatPointForSpent => _personageInfo.RaceInfo != null &&
			_personageInfo.RaceInfo.Race == Race.Human ? 6 + _humanStatPointsBonus : 6;
		private const int _humanStatPointsBonus = 3;

		private IDataSaveLoader _dataSaveLoader;
		private GlobalDataManager _globalDataManager;
		private MessageBoxManager _messageBoxManager;

		private PlayerCustomizer _playerCustomizer => GameData.MainPlayer.PlayerCustomizer;

		private Color _hairColor;
		public Color HairColor
		{
			get => _hairColor;
			set
			{
				if (HairColor != value)
				{
					_hairColor = value;
					_personageCreator.HairColor = value;
					_playerCustomizer.ChangeHairColor(value);
				}
			}
		}

		private Color _skinColor;
		public Color SkinColor
		{
			get => _skinColor;
			set
			{
				if (SkinColor != value)
				{
					_skinColor = value;
					_personageCreator.SkinColor = value;
					_playerCustomizer.ChangeSkinColor(value);
				}
			}
		}

		private Gender _gender;
		public Gender Gender
		{
			get => _gender;
			set
			{
				if (Gender != value)
				{
					_gender = value;
					_personageCreator.SetGenderWithoutNotify(value);
					_playerCustomizer.ChangeGender(Gender);
					_personageInfo.Gender = Gender;
				}
			}
		}

		private CharacteristicRedactorViewModel[] _characteristicRedactors;
		private PersonageCreator _personageCreator;

		internal PersonageCreatorViewModel(PersonageCreator personageCreator, GlobalDataManager globalDataManager, MessageBoxManager messageBoxManager)
		{
			_messageBoxManager = messageBoxManager;
			_dataSaveLoader = GlobalDataManager.DataSaveLoader;
			_globalDataManager = globalDataManager;
			_personageCreator = personageCreator;
			_personageInfo = GameData.MainPlayer.PlayerController.Personage.PersonageInfo;
			_personageInfo.ResetPersonageInfo();

			Color hairColor = _playerCustomizer.GetHairsColor();
			hairColor.a = 1;
			HairColor = hairColor;

			Color skinColor = _playerCustomizer.GetSkinColor();
			skinColor.a = 1;
			SkinColor = skinColor;

			_personageCreator.OnRotate += OnRotate;
			_personageCreator.OnGenderChanged.AddListener(GenderChanged);
			_personageInfo.UnSpendedStatPoints = _maxStatPointForSpent;

			SetName(string.Empty);

			_characteristicRedactors = new CharacteristicRedactorViewModel[_personageCreator.StatRedactors.Length];
			for (int i = 0; i < _personageCreator.StatRedactors.Length; i++)
			{
				_characteristicRedactors[i] = new CharacteristicRedactorViewModel(this, _personageCreator.StatRedactors[i]);
			}
			foreach (var redactor in _personageCreator.ApperanceRedactors)
			{
				redactor.Setup(this);
			}

			SetRace(_globalDataManager.GetRaceInfo(Race.Human));

			_personageCreator.SetRace += SetRace;
			Gender = Gender.Male;
			_personageCreator.OnTrySavePersonage += TrySavePersonage;
			_personageCreator.PickHairColorPressed += OnPickHairColorPressed;
			_personageCreator.PickSkinColorPressed += OnPickSkinColorPressed;
			_personageCreator.OnNameChanged.AddListener(SetName);
		}

		private void OnRotate(float hRotation)
		{
			MaleObject.transform.Rotate(Vector3.up, -hRotation);
			FemaleObject.transform.Rotate(Vector3.up, -hRotation);
		}

		private void RefreshStatPoints()
		{
			_personageCreator.RefreshStatPoints(_maxStatPointForSpent, _personageInfo.UnSpendedStatPoints);
		}

		public void AddStatPoint(Characteristics characteristic)
		{
			if (CanAddMore(characteristic))
			{
				_personageInfo[characteristic]++;
				_personageInfo.UnSpendedStatPoints -= CostOfStatPoint(_personageInfo[characteristic]);
				RefreshStatPoints();
				if (_personageInfo.UnSpendedStatPoints == 0)
				{
					OnNoMoreStatPoints?.Invoke();
				}
			}
		}

		public void RemoveStatPoint(Characteristics characteristic)
		{
			if (CanRemoveMore(characteristic))
			{
				_personageInfo.UnSpendedStatPoints += CostOfStatPoint(_personageInfo[characteristic]);
				_personageInfo[characteristic]--;
				if (_personageInfo.UnSpendedStatPoints <= 2)
				{
					OnGetStatPoints?.Invoke();
				}
				RefreshStatPoints();
			}
		}

		public (int, int) GetStatValue(Characteristics characteristic)
		{
			return (_personageInfo[characteristic], _personageInfo.RaceInfo[characteristic]);
		}

		public bool CanAddMore(Characteristics characteristic)
		{
			return _personageInfo[characteristic] < _maxPointsForStat && _personageInfo.UnSpendedStatPoints >= CostOfStatPoint(_personageInfo[characteristic] + 1);
		}

		public bool CanRemoveMore(Characteristics characteristic)
		{
			return _personageInfo[characteristic] > _minPointsForStat;
		}

		public void SetRace(RaceInfo raceInfo)
		{
			if (_personageInfo.RaceInfo == null || raceInfo.Race != _personageInfo.RaceInfo.Race)
			{
				if (_personageInfo.RaceInfo != null)
				{
					RemoveOldRaceBonus();
				}
				_personageInfo.RaceInfo = raceInfo;
				AddRaceBonus();
				if (_personageInfo.UnSpendedStatPoints < 0)
				{
					ResetStatPoints();
				}
				RefreshStatPoints();
				_personageCreator.UpdateRaceInfo(_personageInfo.RaceInfo);
				foreach (var redactor in _characteristicRedactors)
				{
					redactor.UpdateAmount();
				}
			}
		}

		private void RemoveOldRaceBonus()
		{
			switch (_personageInfo.RaceInfo.Race)
			{
				case Race.Human:
					_personageInfo.UnSpendedStatPoints -= _humanStatPointsBonus;
					break;
			}
		}

		private void AddRaceBonus()
		{
			switch (_personageInfo.RaceInfo.Race)
			{
				case Race.Human:
					if (_personageInfo.UnSpendedStatPoints < _maxStatPointForSpent)
					{
						_personageInfo.UnSpendedStatPoints += _humanStatPointsBonus;
					}
					break;
			}
		}

		private void ResetStatPoints()
		{
			_personageInfo.UnSpendedStatPoints = _maxStatPointForSpent;
			_personageInfo.ResetStats();
			OnGetStatPoints?.Invoke();
			RefreshStatPoints();
			foreach (var redactor in _characteristicRedactors)
			{
				redactor.UpdateAmount();
			}
		}

		public void TrySavePersonage()
		{
			string errors = null;
			if (string.IsNullOrEmpty(_personageInfo.Name))
			{
				errors = "Введите имя персонажа";
			}
			if (_personageInfo.UnSpendedStatPoints > 0)
			{
				if(errors == null)
				{
					errors = "Распределите все очки характеристик";
				}
				else
				{
					errors += "\n" + "Распределите все очки характеристик";
				}
			}

			if (!string.IsNullOrEmpty(errors))
			{
				_messageBoxManager.ShowMessage(errors);
				return;
			}

			ApplyBonuses();
			AppearanceStruct appearance = new AppearanceStruct();
			foreach (var redactor in _personageCreator.ApperanceRedactors)
			{
				switch (redactor.MyAppearancePart)
				{
					case AppearancePart.Hairs:
						appearance.HairIndex = redactor.ActiveIndex;
						break;
					case AppearancePart.Beard:
						appearance.BeardIndex = redactor.ActiveIndex;
						break;
					case AppearancePart.Face:
						appearance.FaceIndex = redactor.ActiveIndex;
						break;
				}
			}
			appearance.HairsColor = HairColor;
			appearance.SkinColor = SkinColor;
			GameData.MainPersonageAppearance = appearance;
			_playerCustomizer.ApplyAppearance(appearance);
			_personageInfo.PersonagePortrait = _personageCreator.SavePersonagePortrait();

			GameData.InitializeNewGame(_personageInfo);
			MaleObject.transform.rotation = Quaternion.identity;
			FemaleObject.transform.rotation = Quaternion.identity;
			_dataSaveLoader.CreateGameSaveInfo(GameData.NewGameSave());
			SceneManager.LoadScene("SampleScene");
		}

		private void ApplyBonuses()
		{
			for (int i = 0; i < Enum.GetValues(typeof(Characteristics)).Length; i++)
			{
				_personageInfo[(Characteristics)i] += _personageInfo.RaceInfo[(Characteristics)i];
			}
		}

		public void SetName(string name)
		{
			_personageInfo.Name = name.Trim();
		}

		private int CostOfStatPoint(int statPointNunber) => 1;

		private void OnPickHairColorPressed()
		{
			_personageCreator.PickedColorChanged.RemoveListener(ChangeSkinColor);
			_personageCreator.ActivateColorPicker(HairColor);
			_personageCreator.PickedColorChanged.AddListener(ChangeHairColor);
		}

		private void ChangeHairColor(Color color)
		{
			HairColor = color;
		}

		private void OnPickSkinColorPressed()
		{
			_personageCreator.PickedColorChanged.RemoveListener(ChangeHairColor);
			_personageCreator.ActivateColorPicker(SkinColor);
			_personageCreator.PickedColorChanged.AddListener(ChangeSkinColor);
		}

		private void ChangeSkinColor(Color color)
		{
			SkinColor = color;
		}

		private void GenderChanged(Gender gender)
		{
			Gender = gender;
		}

		public void Dispose()
		{
			_personageCreator.SetRace -= SetRace;
			_personageCreator.OnRotate -= OnRotate;
		}
	}
}
