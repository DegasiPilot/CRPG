using System;
using UnityEngine;

namespace DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGUID;
        public string DialogueTitle;
        public string DialogueText;
        public Vector2 Position;
    }
}