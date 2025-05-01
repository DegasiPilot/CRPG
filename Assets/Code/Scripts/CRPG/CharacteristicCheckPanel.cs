using DialogueSystem.DataContainers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialogueSystem.Runtime
{
    internal class CharacteristicCheckPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI CharacteristicNameText;
        [SerializeField] private TextMeshProUGUI DifficultyNumberText;
        [SerializeField] private TextMeshProUGUI ResultNumberText;
        [SerializeField] private TextMeshProUGUI ResultText;
        [SerializeField] private TextMeshProUGUI BonusText;
        [SerializeField] private Button ConfirmButton;

        private CharacteristicNodeData _checkNode;
        private NodeLinkData[] _nodeLinkData;
        [SerializeField] private TextMeshProUGUI _confirmBtnText;
        private string _nextNodeGIUD;
        private int _bonus;
        private DialogueParser _dialogueParser;

		private void OnValidate()
		{
			if(_confirmBtnText == null) _confirmBtnText = ConfirmButton.GetComponentInChildren<TextMeshProUGUI>();
		}

        internal void Setup(DialogueParser dialogueParser)
        {
            _dialogueParser = dialogueParser;
        }

        public void CharacteristicCheck(CharacteristicNodeData nodeData, NodeLinkData[] nodeLinks, PersonageInfo personage)
        {
            _checkNode = nodeData;
            _nodeLinkData = nodeLinks;

            string characteristicName = Translator.Translate(_checkNode.Characteristic);
            CharacteristicNameText.text = characteristicName;
            DifficultyNumberText.text = _checkNode.CheckDifficulty.ToString();
            _bonus = personage.GetCharacteristicBonus(nodeData.Characteristic);
            BonusText.text = _bonus >= 0 ? $"Бонус от {characteristicName}: +{_bonus}" : $"Штраф от {characteristicName} {_bonus}";
            _confirmBtnText.text = "Бросить кубик";
        }

        public void ButtonPressed()
        {
            if (string.IsNullOrEmpty(ResultText.text))
            {
                CheckResult checkResult = CharacteristicChecker.Check(_bonus, _checkNode.CheckDifficulty, out int diceResult, out int finalResult);
                ResultNumberText.text = diceResult.ToString();
                ResultText.text = Translator.Translate(checkResult);
                ResultText.color = checkResult < CheckResult.Succes ? Color.red : Color.green;
                _nextNodeGIUD = checkResult >= CheckResult.Succes ? _nodeLinkData[0].TargetNodeGUID :
                    _nodeLinkData[1].TargetNodeGUID;
                _confirmBtnText.text = "Продолжить";
            }
            else
            {
                ResultNumberText.text = string.Empty;
                ResultText.text = string.Empty;
                gameObject.SetActive(false);
                _dialogueParser.ProceedToNarrative(_nextNodeGIUD);
            }
        }
    }
}