using UnityEngine;

namespace MedievalChinaARPG.World
{
    public class HubPortal : MonoBehaviour
    {
        [SerializeField] string targetScene = "CombatLevel";
        [SerializeField] string prompt = "Enter combat grounds [E]";
        bool _playerInside;

        void Update()
        {
            if (_playerInside && Input.GetKeyDown(KeyCode.E))
                SceneLoader.Load(targetScene);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) _playerInside = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) _playerInside = false;
        }

        void OnGUI()
        {
            if (!_playerInside) return;
            var rect = new Rect(Screen.width * 0.5f - 140f, Screen.height * 0.78f, 280f, 36f);
            GUI.Box(rect, prompt);
        }
    }
}
