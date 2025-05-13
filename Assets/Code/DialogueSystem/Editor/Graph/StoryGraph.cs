using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.DataContainers;
using UnityEditor.Callbacks;

namespace DialogueSystem.Editor
{
    public class StoryGraph : EditorWindow
    {
        private string _filePath;

        private StoryGraphView _graphView;

		[OnOpenAsset(1)]
		public static bool OpenGameStateWindow(int instanceID, int line)
		{
            if(EditorUtility.InstanceIDToObject(instanceID) is not DialogueContainer)
            {
                return false;
            }
			bool windowIsOpen = HasOpenInstances<StoryGraph>();

			if (!windowIsOpen)
			{
                CreateGraphViewWindow();
			}
			else
			{
				FocusWindowIfItsOpen<StoryGraph>();
			}

			var window = GetWindow<StoryGraph>();

			string assetPath = AssetDatabase.GetAssetPath(instanceID);
			var saveUtility = GraphSaveUtility.GetInstance(window._graphView);
			window._filePath = assetPath;
			saveUtility.LoadNarrative(assetPath);
			window.RegenerateToolbar();

            return true;
		}

		[MenuItem("Graph/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            CreateWindow<StoryGraph>("Dialogue");
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
            toolbar.Add(new Button(() => RequestDataOperation(DataOperationType.CreateNew)) { text = "New" });
            toolbar.Add(new Button(() => RequestDataOperation(DataOperationType.Save)) { text = "Save" });
            toolbar.Add(new Button(() => RequestDataOperation(DataOperationType.Load)) { text = "Load" });
            if (_filePath != string.Empty)
            {
                var fileNameTextField = new Label($"File Path: {_filePath}");
                toolbar.Add(fileNameTextField);
            }
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(DataOperationType operationType)
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            switch (operationType)
            {
                case DataOperationType.CreateNew:
                    {
                        _filePath = string.Empty;
                        rootVisualElement.Remove(_graphView);
                        ConstructGraphView();
                        RegenerateToolbar();
                        GenerateBlackBoard();
                        break;
                    }
                case DataOperationType.Save: 
                    {
                        if (_filePath != string.Empty && _filePath != null)
                        {
                            saveUtility.SaveGraph(_filePath);
                        }
                        else saveUtility.SaveGraph(out _filePath);

                        Debug.Log($"Saved Narrative at: {_filePath}", AssetDatabase.LoadAssetAtPath<DialogueContainer>(_filePath));
                        RegenerateToolbar();
                        break;
                    }
                case DataOperationType.Load: 
                    {
                        saveUtility.SelectPathForLoad(out _filePath);
                        saveUtility.LoadNarrative(_filePath);
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