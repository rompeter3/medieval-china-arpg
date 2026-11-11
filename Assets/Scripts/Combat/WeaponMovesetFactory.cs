using UnityEngine;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.Combat
{
    /// <summary>
    /// Runtime fallback movesets when ScriptableObject skills are missing.
    /// Ensures the 8 playable weapons always have light/heavy/skill behavior.
    /// </summary>
    public static class WeaponMovesetFactory
    {
        public static void EnsureMoveset(WeaponDefinition weapon)
        {
            if (weapon == null || !weapon.isPlayable) return;
            if (weapon.lightCombo == null)
                weapon.lightCombo = Make($"Light_{weapon.category}", SkillKind.Light, 1f, 0.32f, weapon.range, false, 8f);
            if (weapon.heavyAttack == null)
                weapon.heavyAttack = Make($"Heavy_{weapon.category}", SkillKind.Heavy, 1.65f, 0.55f, weapon.range * 1.15f, false, 16f);
            if (weapon.weaponSkill == null)
                weapon.weaponSkill = CreateWeaponSkill(weapon);
        }

        static SkillDefinition CreateWeaponSkill(WeaponDefinition weapon)
        {
            switch (weapon.category)
            {
                case WeaponCategory.Bow:
                case WeaponCategory.Crossbow:
                case WeaponCategory.ShortBow:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.4f, 0.4f, 1f, true, 22f, 4f);
                case WeaponCategory.ThreeSectionStaff:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.2f, 0.7f, weapon.range * 1.4f, false, 18f, 5f);
                case WeaponCategory.SpearQiang:
                case WeaponCategory.Guandao:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.8f, 0.65f, weapon.range * 1.5f, false, 24f, 6f);
                case WeaponCategory.Katana:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 2.1f, 0.5f, weapon.range * 1.2f, false, 20f, 5.5f);
                case WeaponCategory.Jian:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.5f, 0.45f, weapon.range * 1.1f, false, 16f, 4.5f);
                case WeaponCategory.Dao:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.9f, 0.55f, weapon.range * 1.25f, false, 20f, 5f);
                default:
                    return Make($"Skill_{weapon.category}", SkillKind.WeaponSkill, 1.5f, 0.5f, weapon.range, false, 18f, 5f);
            }
        }

        static SkillDefinition Make(string id, SkillKind kind, float mult, float duration, float radius, bool projectile, float stamina, float cooldown = 0f)
        {
            var skill = ScriptableObject.CreateInstance<SkillDefinition>();
            skill.skillId = id;
            skill.displayName = id.Replace('_', ' ');
            skill.kind = kind;
            skill.damageMultiplier = mult;
            skill.duration = duration;
            skill.windup = kind == SkillKind.Heavy ? 0.18f : 0.08f;
            skill.recovery = 0.12f;
            skill.hitRadius = radius;
            skill.forwardOffset = Mathf.Max(0.8f, radius * 0.65f);
            skill.knockback = kind == SkillKind.Heavy ? 4f : 2f;
            skill.staminaCost = stamina;
            skill.cooldown = cooldown;
            skill.isProjectile = projectile;
            skill.projectileSpeed = projectile ? (id.Contains("Crossbow") ? 28f : 22f) : 0f;
            return skill;
        }
    }
}
