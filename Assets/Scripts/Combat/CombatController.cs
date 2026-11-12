using System.Collections;
using UnityEngine;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.Combat
{
    public class CombatController : MonoBehaviour
    {
        [SerializeField] Equipment equipment;
        [SerializeField] Hitbox hitbox;
        [SerializeField] Transform attackOrigin;
        [SerializeField] float stamina = 100f;
        [SerializeField] float maxStamina = 100f;
        [SerializeField] float staminaRegen = 18f;

        int _comboStep;
        float _comboTimer;
        float _weaponSkillCooldown;
        bool _busy;
        Coroutine _attackRoutine;

        public bool IsBusy => _busy;
        public float Stamina01 => stamina / maxStamina;
        public float WeaponSkillCooldown01 =>
            equipment?.equippedWeapon?.weaponSkill == null || equipment.equippedWeapon.weaponSkill.cooldown <= 0f
                ? 0f
                : Mathf.Clamp01(_weaponSkillCooldown / equipment.equippedWeapon.weaponSkill.cooldown);

        void Awake()
        {
            if (equipment == null) equipment = GetComponent<Equipment>();
            if (hitbox == null) hitbox = GetComponent<Hitbox>();
            if (attackOrigin == null) attackOrigin = transform;
        }

        void Update()
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegen * Time.deltaTime);
            if (_weaponSkillCooldown > 0f) _weaponSkillCooldown -= Time.deltaTime;
            if (_comboTimer > 0f)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0f) _comboStep = 0;
            }
        }

        public bool TryLightAttack()
        {
            var skill = ResolveLight();
            if (skill == null || _busy) return false;
            if (stamina < skill.staminaCost) return false;
            stamina -= skill.staminaCost;
            skill.comboIndex = _comboStep;
            _comboStep = (_comboStep + 1) % 3;
            _comboTimer = 0.85f;
            StartAttack(skill);
            return true;
        }

        public bool TryHeavyAttack()
        {
            var skill = equipment?.equippedWeapon?.heavyAttack;
            if (skill == null || _busy) return false;
            if (stamina < skill.staminaCost) return false;
            stamina -= skill.staminaCost;
            _comboStep = 0;
            StartAttack(skill);
            return true;
        }

        public bool TryWeaponSkill()
        {
            var skill = equipment?.equippedWeapon?.weaponSkill;
            if (skill == null || _busy) return false;
            if (_weaponSkillCooldown > 0f) return false;
            if (stamina < skill.staminaCost) return false;
            stamina -= skill.staminaCost;
            _weaponSkillCooldown = skill.cooldown;
            _comboStep = 0;
            StartAttack(skill);
            return true;
        }

        SkillDefinition ResolveLight()
        {
            return equipment?.equippedWeapon?.lightCombo;
        }

        void StartAttack(SkillDefinition skill)
        {
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            _attackRoutine = StartCoroutine(AttackRoutine(skill));
        }

        IEnumerator AttackRoutine(SkillDefinition skill)
        {
            _busy = true;
            hitbox.BeginSwing();
            yield return new WaitForSeconds(skill.windup);

            float damage = equipment.WeaponDamage * skill.damageMultiplier * (1f + skill.comboIndex * 0.08f);
            Vector3 origin = attackOrigin.position + attackOrigin.forward * skill.forwardOffset + Vector3.up * 1f;

            if (skill.isProjectile)
                SpawnProjectile(origin, attackOrigin.forward, damage, skill);
            else
                hitbox.Pulse(origin, skill.hitRadius, damage, skill.knockback, gameObject);

            float active = Mathf.Max(0.05f, skill.duration - skill.windup - skill.recovery);
            yield return new WaitForSeconds(active);
            yield return new WaitForSeconds(skill.recovery);
            _busy = false;
            _attackRoutine = null;
        }

        void SpawnProjectile(Vector3 origin, Vector3 dir, float damage, SkillDefinition skill)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Projectile";
            go.transform.position = origin;
            go.transform.localScale = Vector3.one * 0.25f;
            var rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                var shader = Shader.Find("MedievalChinaARPG/StylizedEmissive");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.SetColor("_BaseColor", new Color(0.9f, 0.85f, 0.55f));
                    mat.SetColor("_Emission", new Color(1.4f, 1.1f, 0.4f));
                    rend.sharedMaterial = mat;
                }
            }
            Object.Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.5f;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = dir.normalized * skill.projectileSpeed;
            var proj = go.AddComponent<Projectile>();
            proj.Init(damage, skill.knockback, gameObject, skill.hitRadius);
            Object.Destroy(go, 3f);
        }
    }
}
