using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.DataContainers;

namespace DialogueSystem.Editor
{
    public class StoryGraph : EditorWindow
    {
        private string _fileName;
        private string _filePath;

        private StoryGraphView _graphView;
        private DialogueContainer _dialogueContainer;

        [MenuItem("Graph/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<StoryGraph>("Dialogue");
        }

        private void ConstructGraphView()
        {
            _graphView = new StoryGraphView(this)
            {
                name = "Dialogue Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void RegenerateToolbar()
        {
            rootVisualElement.Remove(rootVisualElement.Q<Toolbar>());
            GenerateToolbar();
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.Add(new Button(() => RequestDataOperation(0)) { text = "New" });
            toolbar.Add(new Button(() => RequestDataOperation(1)) { text = "Save" });
            toolbar.Add(new Button(() => RequestDataOperation(2)) { text = "Load" });
            if (_fileName != string.Empty)
            {
                var fileNameTextField = new Label($"File Name: {_fileName}");
                toolbar.Add(fileNameTextField);
            }
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(byte option)
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            switch (option)
            {
                case 0: //New
                    {
                        _fileName = string.Empty;
                        _filePath = string.Empty;
                        rootVisualElement.Remove(_graphView);
                        ConstructGraphView();
                        RegenerateToolbar();
                        GenerateBlackBoard();
                        break;
                    }
                case 1: //Save
                    {
                        if (_filePath != string.Empty && _filePath != null)
                        {
                            saveUtility.SaveGraph(_filePath);
                        }
                        else saveUtility.SaveGraph(out _filePath);

                        Debug.Log($"Saved Narrative at: {_filePath}");
                        _fileName = _filePath.Split('/').Last();
                        _fileName = _fileName[..^6];
                        RegenerateToolbar();
                        break;
                    }
                case 2: //Load
                    {
                        saveUtility.LoadNarrative(out _filePath, out _fileName);
                        RegenerateToolbar();
                        break;
                    }
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateBlackBoard();
        }

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection { title = "Exposed Variables" });
            blackboard.addItemRequested = _ =>
            {
                _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
            };
            blackboard.editTextRequested = (_, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField)element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}