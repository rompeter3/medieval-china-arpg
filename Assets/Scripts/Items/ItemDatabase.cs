using System.Collections.Generic;
using UnityEngine;

namespace MedievalChinaARPG.Items
{
    [CreateAssetMenu(menuName = "MedievalChinaARPG/Items/Item Database", fileName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<WeaponDefinition> weapons = new();
        public List<ArmorDefinition> armor = new();
        public List<MaterialItem> materials = new();

        public WeaponDefinition FindWeapon(string id)
        {
            return weapons.Find(w => w != null && w.itemId == id);
        }

        public WeaponDefinition FindWeapon(WeaponCategory category)
        {
            return weapons.Find(w => w != null && w.category == category);
        }
    }
}
