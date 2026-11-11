using System;
using UnityEngine;

namespace MedievalChinaARPG.Combat
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float armor;
        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        public bool Invulnerable { get; set; }

        public event Action<float, float> HealthChanged;
        public event Action Died;

        void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Configure(float health, float armorValue)
        {
            maxHealth = health;
            armor = armorValue;
            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void SetArmor(float value) => armor = value;

        public bool ApplyDamage(float rawDamage, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (IsDead || Invulnerable) return false;
            float mitigated = Mathf.Max(1f, rawDamage - armor * 0.35f);
            CurrentHealth = Mathf.Max(0f, CurrentHealth - mitigated);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
            if (IsDead) Died?.Invoke();
            return true;
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
