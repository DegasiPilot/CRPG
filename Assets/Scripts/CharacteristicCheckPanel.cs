using DialogueSystem.DataContainers;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.Runtime
{
    internal class CharacteristicCheckPanel : MonoBehaviour
    {
        [SerializeField] private Text CharacteristicCheckText;
        [SerializeField] private Text DifficultyNumberText;
        [SerializeField] private Text ResultNumberText;
        [SerializeField] private Text ResultText;
        [SerializeField] private Text BonusText;
        [SerializeField] private Button ConfirmButton;

        private CharacteristicNodeData _checkNode;
        private NodeLinkData[] _nodeLinkData;
        private Text _confirmBtnText;
        private string _nextNodeGIUD;

        private int _bonus;

        public void Setup()
        {
            _confirmBtnText = ConfirmButton.GetComponentInChildren<Text>();
            _nodeLinkData = new NodeLinkData[2];
        }

        public void CharacteristicCheck(CharacteristicNodeData nodeData, NodeLinkData[] nodeLinks, PersonageInfo personage)
        {
            _checkNode = nodeData;
            _nodeLinkData = nodeLinks;

            string characteristicName = Translator.Translate(_checkNode.Characteristic);
            CharacteristicCheckText.text = characteristicName;
            DifficultyNumberText.text = _checkNode.CheckDifficulty.ToString();
            _bonus = personage.GetCharacteristicBonus(nodeData.Characteristic);
            BonusText.text = _bonus >= 0 ? $"Бонус от {characteristicName}: +{_bonus}" : $"Штраф от {characteristicName} {_bonus}";
            _confirmBtnText.text = "Бросить кубик";
        }

        public void ButtonPressed()
        {
            if (ResultText.text == "")
            {
                CheckResult checkResult = CharacteristicChecker.Check(_bonus, _checkNode.CheckDifficulty);
                ResultText.text = Translator.Translate(checkResult);
                ResultText.color = checkResult < CheckResult.Succes ? Color.red : Color.green;
                _nextNodeGIUD = checkResult < CheckResult.Succes ? _nodeLinkData[0].TargetNodeGUID :
                    _nodeLinkData[1].TargetNodeGUID;
                _confirmBtnText.text = "Продолжить";
            }
            else
            {
                ResultNumberText.text = "";
                ResultText.text = "";
                gameObject.SetActive(false);
                DialogueParser.Instance.ProceedToNarrative(_nextNodeGIUD);
            }
        }
    }
}