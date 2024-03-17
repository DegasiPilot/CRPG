using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
internal class ActionButton : MonoBehaviour
{
    public ActionType MyAction;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(TogglePlayerAction);
    }

    private void TogglePlayerAction()
    {
        CanvasManager.Instance.TogglePlayerAction(MyAction);
    }
}