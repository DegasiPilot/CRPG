using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
internal class ActionButton : MonoBehaviour
{
    public ActionType MyAction;

    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(TogglePlayerAction);
    }

    private void TogglePlayerAction(bool activate)
    {
        CanvasManager.Instance.TogglePlayerAction(MyAction, activate);
    }
}