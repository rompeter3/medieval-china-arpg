using UnityEngine;

namespace MedievalChinaARPG.Items
{
    [CreateAssetMenu(menuName = "MedievalChinaARPG/Items/Material", fileName = "Mat_")]
    public class MaterialItem : ItemDefinition
    {
        [Header("Material")]
        public string materialTag = "scrap";
        public Rarity rarity = Rarity.Common;

        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            Epic
        }

        private void OnValidate()
        {
            itemType = ItemType.Material;
            maxStack = Mathf.Max(maxStack, 99);
        }
    }
}
