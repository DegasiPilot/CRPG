using BattleSystem.ViewModels;
using BattleSystem.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleUIManager : MonoBehaviour
{
	public static BattleUIManager Instance;
	public BattlePanel BattlePanel;
	[SerializeField] private AttackPanelView AttackPanelView;
	private AttackPanelViewModel AttackPanelViewModel;

	private void Awake()
	{
		Instance = this;

		AttackPanelViewModel = new AttackPanelViewModel(AttackPanelView);
		AttackPanelViewModel.OnSelectEnd += InvokeOnAttack;

		BattleManager.OnBattleStartEvent.AddListener(() =>
		{
			gameObject.SetActive(true);
			Setup(BattleManager.ParticipantPersonages);
		});
		BattleManager.OnPersonageJoinBattleEvent.AddListener(BattlePanel.OnPersonageJoinBattle);
		BattleManager.OnBattleEndEvent.AddListener(OnBattleEnd);
		gameObject.SetActive(false);
	}

	public void Setup(List<PersonageController> personages)
	{
		BattlePanel.Setup(personages);
	}

	public void SetNextActivePersonage()
	{
		BattlePanel.SetNextActivePersonage();
	}

	public void EndTurnBtn_Click()
	{
		BattleManager.PlayerTryEndTurn();
	}

	public void OnBattleEnd()
	{
		BattlePanel.OnBattleEnd();
		gameObject.SetActive(false);
	}

	public void ActivatePlayerActionPanel(Personage player, bool canSkip, bool canAttack, bool needDefend, float coefficient)
	{
		AttackPanelViewModel.Activate(player, canSkip, canAttack, needDefend, coefficient);
	}

	public void DeactivatePlayerActionPanel()
	{
		AttackPanelViewModel.Deactivate();
	}

	private void OnDestroy()
	{
		AttackPanelViewModel.OnSelectEnd -= InvokeOnAttack;
	}

	private void InvokeOnAttack(float attack, float defend)
	{
		AfterPlayerEnergySelection.Invoke(attack, defend);
	}
	[System.NonSerialized] public UnityEvent<float, float> AfterPlayerEnergySelection = new();
}