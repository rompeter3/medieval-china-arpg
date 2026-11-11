using UnityEngine;
using MedievalChinaARPG.Items;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Player;

namespace MedievalChinaARPG.UI
{
    public class InventoryUI : MonoBehaviour
    {
        bool _open;
        bool _upgradeMode;
        Inventory _inventory;
        Equipment _equipment;
        UpgradeSystem _upgrades;
        PlayerController _player;
        Vector2 _scroll;

        void Start()
        {
            BindPlayer();
        }

        void BindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            _inventory = player.GetComponent<Inventory>();
            _equipment = player.GetComponent<Equipment>();
            _upgrades = player.GetComponent<UpgradeSystem>();
            _player = player.GetComponent<PlayerController>();
        }

        void Update()
        {
            if (_inventory == null) BindPlayer();
            if (Input.GetKeyDown(KeyCode.I))
            {
                _open = !_open;
                _upgradeMode = false;
                SetCursorForUi(_open);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _open = true;
                _upgradeMode = true;
                SetCursorForUi(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape) && _open)
            {
                _open = false;
                SetCursorForUi(false);
            }
        }

        void SetCursorForUi(bool ui)
        {
            Cursor.lockState = ui ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = ui;
            if (_player != null) _player.InputLocked = ui;
        }

        void OnGUI()
        {
            if (!_open || _inventory == null) return;
            var box = new Rect(Screen.width - 420, 40, 390, Screen.height - 80);
            GUI.Box(box, _upgradeMode ? "Upgrade" : "Inventory");

            GUILayout.BeginArea(new Rect(box.x + 12, box.y + 28, box.width - 24, box.height - 40));
            if (GUILayout.Button(_upgradeMode ? "Switch to Equip" : "Switch to Upgrade"))
                _upgradeMode = !_upgradeMode;

            if (_equipment?.equippedWeapon != null)
                GUILayout.Label($"Equipped: {_equipment.equippedWeapon.displayName} +{_equipment.weaponTier}  DMG {_equipment.WeaponDamage:0}");

            if (_upgradeMode)
            {
                if (_equipment?.equippedWeapon != null && GUILayout.Button($"Upgrade weapon ({_equipment.equippedWeapon.upgradeMaterialCost} mats)"))
                    _upgrades?.TryUpgradeWeapon();
                if (_equipment?.chest != null && GUILayout.Button("Upgrade chest"))
                    _upgrades?.TryUpgradeArmor(ArmorSlot.Chest);
                if (_equipment?.head != null && GUILayout.Button("Upgrade head"))
                    _upgrades?.TryUpgradeArmor(ArmorSlot.Head);
            }

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < _inventory.Slots.Count; i++)
            {
                var slot = _inventory.Slots[i];
                if (slot.IsEmpty) continue;
                string label = $"{slot.item.displayName} x{slot.count}";
                if (slot.item is WeaponDefinition w)
                    label += w.isPlayable ? $"  [playable +{slot.upgradeTier}]" : "  [stub]";
                if (slot.item is ArmorDefinition a)
                    label += $"  [{a.slot} +{slot.upgradeTier}]";

                GUILayout.BeginHorizontal();
                GUILayout.Label(label, GUILayout.Width(250));
                if (!_upgradeMode)
                {
                    if (slot.item is WeaponDefinition weapon && weapon.isPlayable)
                    {
                        if (GUILayout.Button("Equip", GUILayout.Width(70)))
                        {
                            WeaponMovesetFactory.EnsureMoveset(weapon);
                            _equipment.EquipWeapon(weapon, slot.upgradeTier, _inventory);
                        }
                    }
                    else if (slot.item is ArmorDefinition armor)
                    {
                        if (GUILayout.Button("Equip", GUILayout.Width(70)))
                        {
                            _equipment.EquipArmor(armor, slot.upgradeTier, _inventory);
                            var custom = _equipment.GetComponent<CharacterCustomization>();
                            custom?.ApplyArmorTint(armor.tint);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
