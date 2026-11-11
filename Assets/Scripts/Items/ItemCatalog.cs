using UnityEngine;

namespace MedievalChinaARPG.Items
{
    /// <summary>
    /// Builds the full 20-weapon catalog (+ armor/mats) at runtime when YAML assets are unavailable.
    /// </summary>
    public static class ItemCatalog
    {
        public static ItemDatabase Active;

        public static ItemDatabase BuildRuntimeDatabase()
        {
            var db = ScriptableObject.CreateInstance<ItemDatabase>();

            var scrap = Mat("mat_iron_scrap", "Iron Scrap", MaterialItem.Rarity.Common);
            var jade = Mat("mat_spirit_jade", "Spirit Jade", MaterialItem.Rarity.Rare);
            var silk = Mat("mat_silk_thread", "Silk Thread", MaterialItem.Rarity.Uncommon);
            db.materials.Add(scrap);
            db.materials.Add(jade);
            db.materials.Add(silk);

            // 8 playable
            db.weapons.Add(Weapon("weapon_katana", "Katana", WeaponCategory.Katana, true, 24f, 1.15f, 1.9f, scrap));
            db.weapons.Add(Weapon("weapon_jian", "Jian", WeaponCategory.Jian, true, 20f, 1.25f, 1.8f, scrap));
            db.weapons.Add(Weapon("weapon_dao", "Dao", WeaponCategory.Dao, true, 26f, 1.05f, 2.0f, scrap));
            db.weapons.Add(Weapon("weapon_bow", "Composite Bow", WeaponCategory.Bow, true, 18f, 1.0f, 12f, silk));
            db.weapons.Add(Weapon("weapon_crossbow", "Crossbow", WeaponCategory.Crossbow, true, 28f, 0.75f, 14f, scrap));
            db.weapons.Add(Weapon("weapon_staff3", "Three-Section Staff", WeaponCategory.ThreeSectionStaff, true, 19f, 1.2f, 2.4f, silk));
            db.weapons.Add(Weapon("weapon_qiang", "Qiang Spear", WeaponCategory.SpearQiang, true, 23f, 1.1f, 2.8f, scrap));
            db.weapons.Add(Weapon("weapon_guandao", "Guandao", WeaponCategory.Guandao, true, 32f, 0.85f, 3.0f, jade));

            // 12 stubs
            db.weapons.Add(Stub("weapon_twin_hooks", "Twin Hooks", WeaponCategory.TwinHooks, 22f, 1.2f, 1.7f, "Dual-hook catch-and-slash kit."));
            db.weapons.Add(Stub("weapon_rope_dart", "Rope Dart", WeaponCategory.RopeDart, 17f, 1.3f, 5f, "Flexible reach pressure tool."));
            db.weapons.Add(Stub("weapon_war_fan", "War Fan", WeaponCategory.WarFan, 15f, 1.35f, 1.5f, "Concealed blades, quick feints."));
            db.weapons.Add(Stub("weapon_butterfly", "Butterfly Swords", WeaponCategory.ButterflySwords, 18f, 1.4f, 1.4f, "Close-quarters paired blades."));
            db.weapons.Add(Stub("weapon_nine_ring", "Nine-Ring Saber", WeaponCategory.NineRingSaber, 27f, 0.95f, 2.1f, "Heavy ringing dao with disrupt hits."));
            db.weapons.Add(Stub("weapon_meteor", "Meteor Hammer", WeaponCategory.MeteorHammer, 25f, 0.9f, 4.5f, "Weighted chain smash."));
            db.weapons.Add(Stub("weapon_deer_horn", "Deer Horn Knives", WeaponCategory.DeerHornKnives, 16f, 1.45f, 1.3f, "Trapping knife style."));
            db.weapons.Add(Stub("weapon_monk_spade", "Monk Spade", WeaponCategory.MonkSpade, 30f, 0.8f, 2.7f, "Temple polearm with crescent blade."));
            db.weapons.Add(Stub("weapon_sleeve", "Hidden Sleeve Knives", WeaponCategory.HiddenSleeveKnives, 14f, 1.5f, 1.2f, "Assassin burst tools."));
            db.weapons.Add(Stub("weapon_chain_whip", "Chain Whip", WeaponCategory.ChainWhip, 19f, 1.15f, 3.8f, "Segmented whip control."));
            db.weapons.Add(Stub("weapon_short_bow", "Short Bow", WeaponCategory.ShortBow, 16f, 1.2f, 10f, "Mobile skirmish bow."));
            db.weapons.Add(Stub("weapon_pu_dao", "Pu Dao Glaive", WeaponCategory.GlaivePuDao, 29f, 0.9f, 2.6f, "Infantry glaive sweeps."));

            db.armor.Add(Armor("armor_head_bamboo", "Bamboo Cap", ArmorSlot.Head, 4f, new Color(0.45f, 0.5f, 0.3f), scrap));
            db.armor.Add(Armor("armor_chest_robe", "Traveler Robe", ArmorSlot.Chest, 8f, new Color(0.5f, 0.15f, 0.12f), silk));
            db.armor.Add(Armor("armor_legs_wraps", "Cloth Wraps", ArmorSlot.Legs, 5f, new Color(0.3f, 0.28f, 0.24f), scrap));
            db.armor.Add(Armor("armor_hands_guards", "Wrist Guards", ArmorSlot.Hands, 3f, new Color(0.4f, 0.32f, 0.22f), scrap));
            db.armor.Add(Armor("armor_chest_lamellar", "Lamellar Vest", ArmorSlot.Chest, 14f, new Color(0.35f, 0.35f, 0.4f), jade));

            return db;
        }

        static MaterialItem Mat(string id, string name, MaterialItem.Rarity rarity)
        {
            var m = ScriptableObject.CreateInstance<MaterialItem>();
            m.itemId = id;
            m.displayName = name;
            m.itemType = ItemType.Material;
            m.maxStack = 99;
            m.rarity = rarity;
            m.materialTag = id;
            m.description = $"{name} used for upgrades.";
            return m;
        }

        static WeaponDefinition Weapon(string id, string name, WeaponCategory cat, bool playable, float dmg, float speed, float range, MaterialItem mat)
        {
            var w = ScriptableObject.CreateInstance<WeaponDefinition>();
            w.itemId = id;
            w.displayName = name;
            w.itemType = ItemType.Weapon;
            w.category = cat;
            w.isPlayable = playable;
            w.baseDamage = dmg;
            w.attackSpeed = speed;
            w.range = range;
            w.upgradeMaterial = mat;
            w.upgradeMaterialCost = 3;
            w.description = $"{name} — playable vertical-slice moveset.";
            return w;
        }

        static WeaponDefinition Stub(string id, string name, WeaponCategory cat, float dmg, float speed, float range, string note)
        {
            var w = Weapon(id, name, cat, false, dmg, speed, range, null);
            w.stubNote = note;
            w.description = $"{name} (stub). {note}";
            return w;
        }

        static ArmorDefinition Armor(string id, string name, ArmorSlot slot, float armor, Color tint, MaterialItem mat)
        {
            var a = ScriptableObject.CreateInstance<ArmorDefinition>();
            a.itemId = id;
            a.displayName = name;
            a.itemType = ItemType.Armor;
            a.slot = slot;
            a.armorValue = armor;
            a.tint = tint;
            a.upgradeMaterial = mat;
            a.upgradeMaterialCost = 2;
            a.description = name;
            return a;
        }
    }
}
