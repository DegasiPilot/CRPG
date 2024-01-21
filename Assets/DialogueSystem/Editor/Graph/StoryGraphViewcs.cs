using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace DialogueSystem.Editor
{
    public class StoryGraphView : GraphView
    {
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();

        public StoryGraphView(StoryGraph editorWindow)
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            AddElement(GetEntryPointNodeInstance());
            this.AddManipulator(CreateNodeContextualMenu("Add Dialogue Node"));
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", "DialogueText", actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = "string" };
            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(string nodeTitle, string nodeName, Vector2 position)
        {
            AddElement(CreateNode(nodeTitle, nodeName, position));
        }

        public DialogueNode CreateNode(string nodeTitle, string nodeText, Vector2 position)
        {
            var node = new DialogueNode()
            {
                DialogueText = nodeText,
                GUID = Guid.NewGuid().ToString()
            };
            TextField titleTextField = new TextField()
            {
                value = nodeTitle
            };
            titleTextField.RegisterValueChangedCallback(evt =>
            {
                node.DialogueTitle = evt.newValue;
            });
            titleTextField.style.maxWidth = 200;
            node.titleContainer.Insert(0,titleTextField);

            var inputPort = GetPortInstance(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);
            node.SetPosition(new Rect(position, Vector2.zero));


            Foldout nodeFoldout = new Foldout()
            {
                text = "Dialogue text"
            };
            var dialogueTextField = new TextField("");
            dialogueTextField.RegisterValueChangedCallback(evt =>
            {
                node.DialogueText = evt.newValue;
            });
            dialogueTextField.style.maxWidth = 450;
            dialogueTextField.multiline = true;
            dialogueTextField.SetValueWithoutNotify(node.DialogueText);
            nodeFoldout.Add(dialogueTextField);
            node.extensionContainer.Add(nodeFoldout);
            node.RefreshExpandedState();

            var button = new Button(() => { AddChoicePort(node); })
            {
                text = "Add Choice"
            };
            node.titleButtonContainer.Add(button);
            return node;
        }


        public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;

            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName,
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            textField.style.maxWidth = 240;
            textField.multiline = true;
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(bool));
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntyPoint = true
            };

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 0, 0));
            return nodeCache;
        }
    }
}