using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using DialogueSystem.DataContainers;
using UnityEngine.UIElements;
using System;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<DialogueNode> Nodes => _graphView.nodes.ToList().Cast<DialogueNode>().ToList();

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
            EditorUtility.RevealInFinder($"{filePath}");
        }

        public void SaveGraph(string filePath)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(dialogueContainerObject))
                return;
            SaveExposedProperties(dialogueContainerObject);
            var loadedAsset = AssetDatabase.LoadAssetAtPath($"{filePath}", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueContainerObject, $"{filePath}");
            }
            else
            {
                var container = loadedAsset as DialogueContainer;
                if (container != null)
                {
                    container.NodeLinks = dialogueContainerObject.NodeLinks;
                    container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                    container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                    EditorUtility.SetDirty(container);
                }
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as DialogueNode);
                var inputNode = (connectedSockets[i].input.node as DialogueNode);
                dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = node.GUID,
                    DialogueTitle = node.DialogueTitle,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        public void LoadNarrative(out string filePath, out string fileName)
        {
            fileName = string.Empty;
            // open file explorer to get file path
            filePath = EditorUtility.OpenFilePanel("Load Narrative", Application.dataPath + "/Resources", "asset");
            if (filePath.Length == 0) 
                return;

            // reduce the file path to only include the path to the file from the Application.dataPath folder
            filePath = filePath.Replace(Application.dataPath, "Assets");
            // find the last / in the file path and get the file name
            var startIndex = filePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
            var endIndex = filePath.LastIndexOf(".asset", StringComparison.Ordinal);
            fileName = filePath.Substring(startIndex, endIndex - startIndex);
            // shorten the file path to only include the path to the file from the Assets folder
            _dialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>(filePath);

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntyPoint).GUID = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
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
                var tempNode = _graphView.CreateNode(perNode.DialogueTitle, perNode.DialogueText, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), targetNode.inputContainer[0].Q<Port>());

                    targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position, Vector2.zero));
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