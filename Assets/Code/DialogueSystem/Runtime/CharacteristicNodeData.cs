using System;

namespace DialogueSystem.DataContainers
{
	[Serializable]
	public class CharacteristicNodeData : SaveableNodeData
	{
		public Characteristics Characteristic;
		public int CheckDifficulty;

		public override NodeType NodeType => NodeType.CharacteristicCheck;
	}
}