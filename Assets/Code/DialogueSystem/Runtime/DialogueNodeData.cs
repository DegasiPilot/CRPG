using System;

namespace DialogueSystem.DataContainers
{
	[Serializable]
	public class DialogueNodeData : SaveableNodeData
	{
		public string DialogueTitle;
		public string DialogueText;

		public override NodeType NodeType => NodeType.Dialogue;
	}
}