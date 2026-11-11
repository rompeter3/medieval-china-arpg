using UnityEngine;
using MedievalChinaARPG.Combat;

namespace MedievalChinaARPG.Items
{
    public enum WeaponCategory
    {
        Katana,
        Jian,
        Dao,
        Bow,
        Crossbow,
        ThreeSectionStaff,
        SpearQiang,
        Guandao,
        TwinHooks,
        RopeDart,
        WarFan,
        ButterflySwords,
        NineRingSaber,
        MeteorHammer,
        DeerHornKnives,
        MonkSpade,
        HiddenSleeveKnives,
        ChainWhip,
        ShortBow,
        GlaivePuDao
    }

    [CreateAssetMenu(menuName = "MedievalChinaARPG/Items/Weapon", fileName = "Weapon_")]
    public class WeaponDefinition : ItemDefinition
    {
        [Header("Weapon")]
        public WeaponCategory category;
        public bool isPlayable = true;
        public float baseDamage = 20f;
        public float attackSpeed = 1f;
        public float range = 1.8f;
        public float staminaCost = 8f;
        public int upgradeTier = 0;
        public int maxUpgradeTier = 5;
        public float damagePerTier = 4f;
        public MaterialItem upgradeMaterial;
        public int upgradeMaterialCost = 3;

        [Header("Moveset")]
        public SkillDefinition lightCombo;
        public SkillDefinition heavyAttack;
        public SkillDefinition weaponSkill;
        public string stubNote;

        public float GetDamageAtTier(int tier)
        {
            return baseDamage + damagePerTier * Mathf.Clamp(tier, 0, maxUpgradeTier);
        }
    }
}
