using UnityEngine;

namespace MedievalChinaARPG.Combat
{
    public enum SkillKind
    {
        Light,
        Heavy,
        WeaponSkill,
        Dodge
    }

    [CreateAssetMenu(menuName = "MedievalChinaARPG/Combat/Skill", fileName = "Skill_")]
    public class SkillDefinition : ScriptableObject
    {
        public string skillId;
        public string displayName;
        public SkillKind kind;
        public float damageMultiplier = 1f;
        public float duration = 0.35f;
        public float windup = 0.08f;
        public float recovery = 0.12f;
        public float hitRadius = 1.2f;
        public float forwardOffset = 1.0f;
        public float knockback = 2f;
        public float staminaCost = 10f;
        public float cooldown = 0f;
        public int comboIndex;
        public bool isProjectile;
        public float projectileSpeed = 18f;
        public string animationTrigger = "Attack";
    }
}
