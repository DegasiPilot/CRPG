

namespace CRPG.Battle
{
    class FigtherInfo
    {
        public PersonageController PersonageController;
        public float EnergyToAttack = float.NegativeInfinity;
        public float EnergyToDefend = float.NegativeInfinity;
        public bool IsReady => EnergyToAttack >= 0 && EnergyToDefend >= 0;
    }
}
