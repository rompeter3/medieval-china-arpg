using System.IO;
using UnityEngine;
using MedievalChinaARPG.Player;

namespace MedievalChinaARPG.World
{
    /// <summary>
    /// Unattended tour: Temp/AUTO_DEMO advances CharacterCreate → Hub → Combat with status markers.
    /// </summary>
    public class AutoDemoTour : MonoBehaviour
    {
        float _t;
        string _phase = "boot";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "..", "Temp", "AUTO_DEMO")))
                return;
            if (Object.FindFirstObjectByType<AutoDemoTour>() != null) return;
            var go = new GameObject("AutoDemoTour");
            DontDestroyOnLoad(go);
            go.AddComponent<AutoDemoTour>();
        }

        void Update()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "..", "Temp", "AUTO_DEMO")))
                return;

            _t += Time.unscaledDeltaTime;
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            // Keep milestone markers sticky so external capture can poll them.
            if (_phase != "hub-ready" && _phase != "combat-ready")
                Write($"phase={_phase};scene={scene};t={_t:0.0}");

            if (scene.Contains("Character") && _phase == "boot" && _t > 2.5f)
            {
                PlayerSession.Created = true;
                PlayerSession.StartingWeaponId = "weapon_katana";
                _phase = "to-hub";
                _t = 0f;
                SceneLoader.LoadHub();
            }
            else if (scene.Contains("Hub") && (_phase == "to-hub" || _phase == "boot") && _t > 3f)
            {
                _phase = "hub-ready";
                Write("HUB_READY");
                _t = 0f;
            }
            else if (scene.Contains("Hub") && _phase == "hub-ready" && _t > 3f)
            {
                _phase = "to-combat";
                _t = 0f;
                SceneLoader.LoadCombat();
            }
            else if (scene.Contains("Combat") && _phase == "to-combat" && _t > 3f)
            {
                _phase = "combat-ready";
                Write("COMBAT_READY");
            }
        }

        static void Write(string msg)
        {
            try
            {
                File.WriteAllText(Path.Combine(Application.dataPath, "..", "Temp", "AUTO_DEMO_STATUS.txt"), msg);
            }
            catch { /* ignore */ }
        }
    }
}
