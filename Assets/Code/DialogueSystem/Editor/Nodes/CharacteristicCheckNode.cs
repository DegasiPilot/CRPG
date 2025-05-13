

namespace DialogueSystem.Editor
{
    public class CharacteristicCheckNode : SaveableNode
    {
        public Characteristics Characteristic;
        public int DifficultyNumber;

        public override NodeType NodeType => NodeType.CharacteristicCheck;
    }
}
