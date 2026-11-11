using UnityEngine;

namespace MedievalChinaARPG.Items
{
    [CreateAssetMenu(menuName = "MedievalChinaARPG/Items/Armor", fileName = "Armor_")]
    public class ArmorDefinition : ItemDefinition
    {
        [Header("Armor")]
        public ArmorSlot slot;
        public float armorValue = 5f;
        public Color tint = Color.white;
        public int upgradeTier = 0;
        public int maxUpgradeTier = 5;
        public float armorPerTier = 2f;
        public MaterialItem upgradeMaterial;
        public int upgradeMaterialCost = 2;

        public float GetArmorAtTier(int tier)
        {
            return armorValue + armorPerTier * Mathf.Clamp(tier, 0, maxUpgradeTier);
        }
    }
}
