using UnityEngine;

namespace MedievalChinaARPG.Items
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Material,
        Consumable
    }

    public enum ArmorSlot
    {
        Head,
        Chest,
        Legs,
        Hands
    }

    public abstract class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string itemId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public ItemType itemType;
        public int maxStack = 1;
        public int sellValue = 10;
    }
}
