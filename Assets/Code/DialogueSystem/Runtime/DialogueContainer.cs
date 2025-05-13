using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.DataContainers
{
	[Serializable]
	public class DialogueContainer : ScriptableObject
	{
		public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
		public string StartNodeGuid;
		public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
		public List<CharacteristicNodeData> CharacteristicNodeData = new List<CharacteristicNodeData>();
		public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
	}
}