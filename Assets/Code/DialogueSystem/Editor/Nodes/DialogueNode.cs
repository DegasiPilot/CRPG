namespace DialogueSystem.Editor
{
    public class DialogueNode : SaveableNode
    {
        public string DialogueTitle;
        public string DialogueText;

        public override NodeType NodeType => NodeType.Dialogue;
    }
}