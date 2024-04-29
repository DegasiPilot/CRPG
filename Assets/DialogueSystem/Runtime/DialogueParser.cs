using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem.DataContainers;
using System.Collections.Generic;

namespace DialogueSystem.Runtime
{
    public class DialogueParser : MonoBehaviour
    {
        public static DialogueParser Instance;

        [SerializeField] private Text _dialogueText;
        [SerializeField] private Button _choicePrefab;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private CharacteristicCheckPanel _characteristicCheckPanel;
        [SerializeField] private Button _endDialogueButton;

        public PersonageInfo PlayerPersonageInfo => GameManager.Instance.PlayerPersonage.PersonageInfo;
        [HideInInspector] public DialogueActor SecondDialogueActor;

        private readonly HashSet<SaveableNodeData> allNodes= new();
        private GameObject _dialogueDisplay;
        private DialogueContainer _dialogue;

        private void Awake()
        {
            Instance = this;
        }

        public void Setup()
        {
            _characteristicCheckPanel.Setup();
            _dialogueDisplay = _dialogueText.transform.parent.gameObject;
        }

        public void SetSecondDialogueActor(DialogueActor dialogueActor)
        {
            SecondDialogueActor = dialogueActor;
            _dialogue = dialogueActor.Dialogue;
        }

        public void TryStartDialogue()
        {
            if (_dialogue)
            {
                allNodes.Clear();
                allNodes.UnionWith(_dialogue.DialogueNodeData);
                allNodes.UnionWith(_dialogue.CharacteristicNodeData);
                var narrativeData = _dialogue.NodeLinks.First();
                _endDialogueButton.gameObject.SetActive(false);
                _dialogueDisplay.SetActive(true);
                SecondDialogueActor.OnStartDialogue();
                ProceedToNarrative(narrativeData.TargetNodeGUID); //Entrypoint node
            }
        }

        public void ProceedToNarrative(string narrativeDataGUID)
        {
            SaveableNodeData nodeData = allNodes.First(x => x.NodeGUID == narrativeDataGUID);
            var choices = _dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID).ToArray();
            if (nodeData.NodeType == NodeType.Dialogue)
            {
                _characteristicCheckPanel.gameObject.SetActive(false);
                _dialogueDisplay.SetActive(true);

                var _dialogueNode = nodeData as DialogueNodeData;
                string text = _dialogueNode.DialogueText;
                _dialogueText.text = SecondDialogueActor.PersonageInfo.Name + ": " + ProcessProperties(text);
                var buttons = _buttonContainer.GetComponentsInChildren<Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    Destroy(buttons[i].gameObject);
                }

                for (int i = 0; i < choices.Count(); i++)
                {
                    var button = Instantiate(_choicePrefab, _buttonContainer);
                    button.GetComponentInChildren<Text>().text = ProcessProperties($"{i + 1})" + choices[i].PortName);
                    var targetNodeGUID = choices[i].TargetNodeGUID;
                    button.onClick.AddListener(() => ProceedToNarrative(targetNodeGUID));
                }
            }
            else if(nodeData.NodeType == NodeType.CharacteristicCheck)
            {
                _dialogueDisplay.SetActive(false);

                var checkNode = nodeData as CharacteristicNodeData;

                _characteristicCheckPanel.gameObject.SetActive(true);
                _characteristicCheckPanel.CharacteristicCheck(checkNode, choices, PlayerPersonageInfo);
            }

            if(choices.Length == 0)
            {
                _endDialogueButton.gameObject.SetActive(true);
            }
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in _dialogue.ExposedProperties)
            {
                if(exposedProperty.PropertyName == "PlayerName")
                {
                    exposedProperty.PropertyValue = PlayerPersonageInfo.Name;
                }
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            }
            return text;
        }

        public void EndDialogue()
        {
            _dialogueDisplay.SetActive(false);
            _dialogue = null;
            SecondDialogueActor.OnEndDialogue();
            GameManager.Instance.ChangeGameMode(GameMode.Free);
        }
    }
}