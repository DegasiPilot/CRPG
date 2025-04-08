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
		AttackPanelViewModel.OnAttack += InvokeOnAttack;

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
        BattleManager.TryEndTurn();
    }

    public void OnBattleEnd()
    {
        BattlePanel.OnBattleEnd();
        gameObject.SetActive(false);
    }

	public void ActivatePlayerActionPanel(Personage player)
	{
		AttackPanelViewModel.Activate(player);
	}

	public void DeactivatePlayerActionPanel()
	{
		AttackPanelViewModel.Deactivate();
	}

	private void OnDestroy()
	{
		AttackPanelViewModel.OnAttack -= InvokeOnAttack;
	}

	private void InvokeOnAttack(float force)
	{
		AfterPlayerAttack.Invoke(force);
	}
	[System.NonSerialized] public UnityEvent<float> AfterPlayerAttack = new();
}