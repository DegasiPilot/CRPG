using System;
using UnityEngine;

namespace DialogueSystem.DataContainers
{
    [Serializable]
    public abstract class SaveableNodeData
    {
        public string NodeGUID;
        public Vector2 Position;

        public abstract NodeType NodeType { get; }
    }
}