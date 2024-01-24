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
        public Personage PlayerPersonage;
        public Personage SecondPersonage;

        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CharacteristicCheckPanel characteristicCheckPanel;

        private readonly HashSet<SaveableNodeData> allNodes= new();
        private GameObject dialogueDisplay;

        public void Setup()
        {
            Instance = this;
            characteristicCheckPanel.Setup();
            dialogueDisplay = dialogueText.transform.parent.gameObject;
        }

        public void StartDialogue()
        {
            allNodes.Clear();
            allNodes.UnionWith(dialogue.DialogueNodeData);
            allNodes.UnionWith(dialogue.CharacteristicNodeData);
            var narrativeData = dialogue.NodeLinks.First(); 
            ProceedToNarrative(narrativeData.TargetNodeGUID); //Entrypoint node
        }

        public void ProceedToNarrative(string narrativeDataGUID)
        {
            SaveableNodeData nodeData = allNodes.First(x => x.NodeGUID == narrativeDataGUID);
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID).ToArray();
            if (nodeData.NodeType == NodeType.Dialogue)
            {
                characteristicCheckPanel.gameObject.SetActive(false);
                dialogueDisplay.SetActive(true);

                var dialogueNode = nodeData as DialogueNodeData;
                string text = dialogueNode.DialogueText;
                dialogueText.text = SecondPersonage.Name + ": " + ProcessProperties(text);
                var buttons = buttonContainer.GetComponentsInChildren<Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    Destroy(buttons[i].gameObject);
                }

                for (int i = 0; i < choices.Count(); i++)
                {
                    var button = Instantiate(choicePrefab, buttonContainer);
                    button.GetComponentInChildren<Text>().text = ProcessProperties($"{i + 1})" + choices[i].PortName);
                    var targetNodeGUID = choices[i].TargetNodeGUID;
                    button.onClick.AddListener(() => ProceedToNarrative(targetNodeGUID));
                }
            }
            else if(nodeData.NodeType == NodeType.CharacteristicCheck)
            {
                dialogueDisplay.SetActive(false);

                var checkNode = nodeData as CharacteristicNodeData;

                dialogueDisplay.SetActive(false);
                characteristicCheckPanel.gameObject.SetActive(true);
                characteristicCheckPanel.CharacteristicCheck(checkNode, choices, PlayerPersonage);
            }
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            }
            return text;
        }
    }
}