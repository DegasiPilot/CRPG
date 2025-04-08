using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor
{
    public abstract class SaveableNode : Node
    {
        public string GUID;
        public bool EntyPoint = false;
        public abstract NodeType NodeType { get; }
    }
}