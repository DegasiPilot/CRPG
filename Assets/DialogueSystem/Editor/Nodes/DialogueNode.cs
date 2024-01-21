using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public string DialogueTitle;
        public string DialogueText;
        public string GUID;
        public bool EntyPoint = false;
    }
}