using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using DialogueSystem.DataContainers;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<SaveableNode> Nodes => _graphView.nodes.ToList().Cast<SaveableNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private StoryGraphView _graphView;

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph() => SaveGraph(out _);

        public void SaveGraph(out string filePath)
        {
            filePath = EditorUtility.SaveFilePanelInProject("Save Narrative", "New Narrative", "asset", "Pick a save location");
            if (string.IsNullOrEmpty(filePath))
                return;

            SaveGraph(filePath);
        }

        public void SaveGraph(string filePath)
        {
			var dialogueContainerObject = AssetDatabase.LoadAssetAtPath<DialogueContainer>(filePath);

			if (dialogueContainerObject != null && AssetDatabase.Contains(dialogueContainerObject))
			{
                AssetDatabase.DeleteAsset(filePath);
			}

			dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();

			if (!SaveNodes(dialogueContainerObject))
                return;
            SaveExposedProperties(dialogueContainerObject);

			AssetDatabase.CreateAsset(dialogueContainerObject, $"{filePath}");
			AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private bool SaveNodes(DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = connectedSockets[i].output.node as SaveableNode;
                var inputNode = connectedSockets[i].input.node as SaveableNode;
                if (outputNode.EntyPoint)
                {
                    dialogueContainerObject.StartNodeGuid = inputNode.GUID;
                }
                else
                {
					dialogueContainerObject.NodeLinks.Add(new NodeLinkData
					{
						BaseNodeGUID = outputNode.GUID,
						PortName = connectedSockets[i].output.portName,
						TargetNodeGUID = inputNode.GUID
					});
				}
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                if (node.NodeType == NodeType.Dialogue)
                {
                    DialogueNode dialogueNode = node as DialogueNode;
                    dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                    {
                        NodeGUID = dialogueNode.GUID,
                        DialogueTitle = dialogueNode.DialogueTitle,
                        DialogueText = dialogueNode.DialogueText,
                        Position = dialogueNode.GetPosition().position
                    });
                }
                else if (node.NodeType == NodeType.CharacteristicCheck)
                {
                    CharacteristicCheckNode characteristicCheckNode = node as CharacteristicCheckNode;
                    dialogueContainerObject.CharacteristicNodeData.Add(new CharacteristicNodeData
                    {
                        NodeGUID = characteristicCheckNode.GUID,
                        Characteristic = characteristicCheckNode.Characteristic,
                        CheckDifficulty = characteristicCheckNode.DifficultyNumber,
                        Position = characteristicCheckNode.GetPosition().position
                    });
                    Debug.Log(characteristicCheckNode.DifficultyNumber);
                }
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        public void SelectPathForLoad(out string filePath)
        {
			// open file explorer to get file path
			filePath = EditorUtility.OpenFilePanel("Load Narrative", Application.dataPath + "/ScriptableObjects/Dialogues", "asset");
			if (filePath.Length == 0)
				return;

			// reduce the file path to only include the path to the file from the Application.dataPath folder
			filePath = filePath.Replace(Application.dataPath, "Assets");
		}

        public void LoadNarrative(string filePath)
        {
            // shorten the file path to only include the path to the file from the Assets folder
            _dialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>(filePath);

            ClearGraph();
            GenerateDialogueNodes();
            GenerateCharacteristicNodes();
            ConnectNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
			foreach (var perNode in Nodes)
            {
                if (perNode.EntyPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateDialogueNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateDialogueNode(perNode.DialogueTitle, perNode.DialogueText, perNode.Position);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void GenerateCharacteristicNodes()
        {
            foreach (var perNode in _dialogueContainer.CharacteristicNodeData)
            {
                var tempNode = _graphView.CreateCharacteristicNode(perNode.Characteristic, perNode.CheckDifficulty, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].GUID == _dialogueContainer.StartNodeGuid)
                {
					LinkNodesTogether(Nodes.First(x => x.EntyPoint).outputContainer[0].Q<Port>(), Nodes[i].inputContainer[0].Q<Port>());
				}
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);

                    int outputIndex = j;
                    if (connections[j].PortName == "Успех")
                    {
                        j = 0;
                    }
                    else if (connections[j].PortName == "Провал")
                    {
                        j = 1;
                    }

                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), targetNode.inputContainer[0].Q<Port>());

					SaveableNodeData saveableNode = _dialogueContainer.DialogueNodeData.FirstOrDefault(x => x.NodeGUID == targetNodeGUID);
                    saveableNode ??= _dialogueContainer.CharacteristicNodeData.FirstOrDefault(x => x.NodeGUID == targetNodeGUID);
                    targetNode.SetPosition(new Rect(saveableNode.Position, Vector2.zero));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }
        }
    }
}