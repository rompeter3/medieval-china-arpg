using UnityEngine;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Items;
using MedievalChinaARPG.AI;
using MedievalChinaARPG.Player;

namespace MedievalChinaARPG.UI
{
    public class HUD : MonoBehaviour
    {
        Damageable _playerHp;
        CombatController _combat;
        Equipment _equipment;
        BossAI _boss;

        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerHp = player.GetComponent<Damageable>();
                _combat = player.GetComponent<CombatController>();
                _equipment = player.GetComponent<Equipment>();
            }
            _boss = Object.FindFirstObjectByType<BossAI>();
        }

        void OnGUI()
        {
            DrawBar(new Rect(24, 20, 260, 22), _playerHp != null ? _playerHp.CurrentHealth / _playerHp.MaxHealth : 0f, Color.red, "HP");
            DrawBar(new Rect(24, 48, 260, 16), _combat != null ? _combat.Stamina01 : 0f, new Color(0.3f, 0.7f, 1f), "STA");

            string weapon = _equipment?.equippedWeapon != null
                ? $"{_equipment.equippedWeapon.displayName} +{_equipment.weaponTier}"
                : "Unarmed";
            GUI.Label(new Rect(24, 72, 400, 24), $"Weapon: {weapon}");
            GUI.Label(new Rect(24, 94, 520, 24), "LMB light | RMB heavy | Q skill | Space dodge | I inventory | Tab upgrade");

            if (_boss != null)
            {
                var dmg = _boss.GetComponent<Damageable>();
                if (dmg != null && !dmg.IsDead)
                {
                    DrawBar(new Rect(Screen.width * 0.5f - 220, 24, 440, 24), dmg.CurrentHealth / dmg.MaxHealth, new Color(0.75f, 0.2f, 0.15f), $"Boss — {_boss.Phase}");
                    if (_boss.IsTelegraphing)
                        GUI.Label(new Rect(Screen.width * 0.5f - 80, 52, 200, 24), "TELEGRAPH!");
                }
            }
        }

        static void DrawBar(Rect rect, float fill, Color color, string label)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = color;
            var fillRect = new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * Mathf.Clamp01(fill), rect.height - 4);
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(rect, $"  {label}");
        }
    }
}
