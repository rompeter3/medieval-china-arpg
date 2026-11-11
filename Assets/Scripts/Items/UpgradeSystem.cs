using UnityEngine;

namespace MedievalChinaARPG.Items
{
    public class UpgradeSystem : MonoBehaviour
    {
        public Inventory inventory;
        public Equipment equipment;

        public bool TryUpgradeWeapon()
        {
            if (equipment == null || inventory == null) return false;
            var weapon = equipment.equippedWeapon;
            if (weapon == null) return false;
            if (equipment.weaponTier >= weapon.maxUpgradeTier) return false;
            if (weapon.upgradeMaterial == null) return false;
            if (inventory.CountOf(weapon.upgradeMaterial) < weapon.upgradeMaterialCost) return false;

            inventory.Remove(weapon.upgradeMaterial, weapon.upgradeMaterialCost);
            equipment.weaponTier++;
            equipment.NotifyChanged();
            return true;
        }

        public bool TryUpgradeArmor(ArmorSlot slot)
        {
            if (equipment == null || inventory == null) return false;
            ArmorDefinition armor = null;
            int tier = 0;
            switch (slot)
            {
                case ArmorSlot.Head: armor = equipment.head; tier = equipment.headTier; break;
                case ArmorSlot.Chest: armor = equipment.chest; tier = equipment.chestTier; break;
                case ArmorSlot.Legs: armor = equipment.legs; tier = equipment.legsTier; break;
                case ArmorSlot.Hands: armor = equipment.hands; tier = equipment.handsTier; break;
            }
            if (armor == null || tier >= armor.maxUpgradeTier) return false;
            if (armor.upgradeMaterial == null) return false;
            if (inventory.CountOf(armor.upgradeMaterial) < armor.upgradeMaterialCost) return false;

            inventory.Remove(armor.upgradeMaterial, armor.upgradeMaterialCost);
            switch (slot)
            {
                case ArmorSlot.Head: equipment.headTier++; break;
                case ArmorSlot.Chest: equipment.chestTier++; break;
                case ArmorSlot.Legs: equipment.legsTier++; break;
                case ArmorSlot.Hands: equipment.handsTier++; break;
            }
            equipment.NotifyChanged();
            return true;
        }
    }
}
