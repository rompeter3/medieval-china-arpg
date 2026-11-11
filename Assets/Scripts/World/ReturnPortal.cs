using UnityEngine;

namespace MedievalChinaARPG.World
{
    public class ReturnPortal : MonoBehaviour
    {
        bool _inside;

        void Update()
        {
            if (_inside && Input.GetKeyDown(KeyCode.E))
                SceneLoader.LoadHub();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponentInParent<Player.PlayerController>() != null)
                _inside = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponentInParent<Player.PlayerController>() != null)
                _inside = false;
        }

        void OnGUI()
        {
            if (!_inside) return;
            GUI.Box(new Rect(Screen.width * 0.5f - 140f, Screen.height * 0.78f, 280f, 36f), "Return to hub [E]");
        }
    }
}
