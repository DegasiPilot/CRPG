using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    internal class GameDataInitializer : MonoBehaviour
    {
        public RaceInfo[] RaceInfos;

        private void Awake()
        {
            GameData.RaceInfos = RaceInfos;
        }
    }
}
