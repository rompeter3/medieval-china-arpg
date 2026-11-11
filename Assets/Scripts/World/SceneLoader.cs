using UnityEngine;
using UnityEngine.SceneManagement;

namespace MedievalChinaARPG.World
{
    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadHub() => Load("HubTown");
        public static void LoadCombat() => Load("CombatLevel");
        public static void LoadCharacterCreate() => Load("CharacterCreate");
    }
}
