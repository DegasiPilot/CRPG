using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    public BattlePanel BattlePanel;

    private void Awake()
    {
        Instance = this;
        BattleManager.OnBattleStartEvent.AddListener(() =>
        {
            gameObject.SetActive(true);
            Setup(BattleManager.ParticipantPersonages.ToArray());
        });
        BattleManager.OnBattleEndEvent.AddListener(OnBattleEnd);
        gameObject.SetActive(false);
    }

    public void Setup(Personage[] personages)
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
}