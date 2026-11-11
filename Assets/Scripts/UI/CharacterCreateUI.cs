using UnityEngine;
using MedievalChinaARPG.Items;
using MedievalChinaARPG.Player;
using MedievalChinaARPG.World;

namespace MedievalChinaARPG.UI
{
    public class CharacterCreateUI : MonoBehaviour
    {
        CharacterCustomization _custom;
        ItemDatabase _db;
        int _weaponIndex;
        WeaponDefinition[] _playable;

        public void Bind(CharacterCustomization custom, ItemDatabase db)
        {
            _custom = custom;
            _db = db;
            _playable = db.weapons.FindAll(w => w != null && w.isPlayable).ToArray();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnGUI()
        {
            if (_custom == null || _db == null) return;
            var box = new Rect(40, 40, 420, 360);
            GUI.Box(box, "Character Create — Medieval China ARPG");
            GUILayout.BeginArea(new Rect(box.x + 16, box.y + 30, box.width - 32, box.height - 40));

            var preset = _custom.Current;
            GUILayout.Label($"Preset: {preset?.presetName ?? "—"}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("< Face/Hair")) _custom.Cycle(-1);
            if (GUILayout.Button("Face/Hair >")) _custom.Cycle(1);
            GUILayout.EndHorizontal();

            if (_playable != null && _playable.Length > 0)
            {
                _weaponIndex = Mathf.Clamp(_weaponIndex, 0, _playable.Length - 1);
                GUILayout.Space(12);
                GUILayout.Label($"Starting weapon: {_playable[_weaponIndex].displayName}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("< Weapon"))
                    _weaponIndex = (_weaponIndex - 1 + _playable.Length) % _playable.Length;
                if (GUILayout.Button("Weapon >"))
                    _weaponIndex = (_weaponIndex + 1) % _playable.Length;
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            GUILayout.Label("Outfit colors follow the selected preset. Armor tints apply after equip.");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Enter Hub Town", GUILayout.Height(40)))
            {
                PlayerSession.Created = true;
                PlayerSession.Appearance = _custom.Current;
                if (_playable != null && _playable.Length > 0)
                    PlayerSession.StartingWeaponId = _playable[_weaponIndex].itemId;
                SceneLoader.LoadHub();
            }
            GUILayout.EndArea();
        }
    }
}
