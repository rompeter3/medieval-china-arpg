using System;
using UnityEngine;

namespace MedievalChinaARPG.Items
{
    public class Equipment : MonoBehaviour
    {
        public WeaponDefinition equippedWeapon;
        public int weaponTier;
        public ArmorDefinition head;
        public ArmorDefinition chest;
        public ArmorDefinition legs;
        public ArmorDefinition hands;
        public int headTier, chestTier, legsTier, handsTier;

        public event Action Changed;

        public float TotalArmor
        {
            get
            {
                float a = 0f;
                if (head) a += head.GetArmorAtTier(headTier);
                if (chest) a += chest.GetArmorAtTier(chestTier);
                if (legs) a += legs.GetArmorAtTier(legsTier);
                if (hands) a += hands.GetArmorAtTier(handsTier);
                return a;
            }
        }

        public float WeaponDamage => equippedWeapon != null
            ? equippedWeapon.GetDamageAtTier(weaponTier)
            : 10f;

        public bool EquipWeapon(WeaponDefinition weapon, int tier, Inventory inventory)
        {
            if (weapon == null || !weapon.isPlayable) return false;
            if (equippedWeapon != null)
                inventory.Add(equippedWeapon, 1, weaponTier);

            equippedWeapon = weapon;
            weaponTier = tier;
            inventory.Remove(weapon, 1);
            Changed?.Invoke();
            return true;
        }

        public bool EquipArmor(ArmorDefinition armor, int tier, Inventory inventory)
        {
            if (armor == null) return false;
            UnequipSlot(armor.slot, inventory);
            switch (armor.slot)
            {
                case ArmorSlot.Head: head = armor; headTier = tier; break;
                case ArmorSlot.Chest: chest = armor; chestTier = tier; break;
                case ArmorSlot.Legs: legs = armor; legsTier = tier; break;
                case ArmorSlot.Hands: hands = armor; handsTier = tier; break;
            }
            inventory.Remove(armor, 1);
            Changed?.Invoke();
            return true;
        }

        public void UnequipSlot(ArmorSlot slot, Inventory inventory)
        {
            ArmorDefinition current = null;
            int tier = 0;
            switch (slot)
            {
                case ArmorSlot.Head: current = head; tier = headTier; head = null; headTier = 0; break;
                case ArmorSlot.Chest: current = chest; tier = chestTier; chest = null; chestTier = 0; break;
                case ArmorSlot.Legs: current = legs; tier = legsTier; legs = null; legsTier = 0; break;
                case ArmorSlot.Hands: current = hands; tier = handsTier; hands = null; handsTier = 0; break;
            }
            if (current != null)
                inventory.Add(current, 1, tier);
            Changed?.Invoke();
        }

        public void NotifyChanged() => Changed?.Invoke();
    }
}
