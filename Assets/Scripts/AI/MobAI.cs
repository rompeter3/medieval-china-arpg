using UnityEngine;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.AI
{
    public enum MobKind
    {
        MeleeBandit,
        RangedArcher
    }

    public class MobAI : MonoBehaviour
    {
        [SerializeField] MobKind kind = MobKind.MeleeBandit;
        [SerializeField] float moveSpeed = 3.2f;
        [SerializeField] float attackRange = 1.6f;
        [SerializeField] float attackCooldown = 1.4f;
        [SerializeField] float aggroRange = 14f;
        [SerializeField] float damage = 12f;
        [SerializeField] Damageable damageable;
        [SerializeField] MaterialItem lootMaterial;
        [SerializeField] int lootCount = 2;

        Transform _player;
        float _cooldown;
        CharacterController _cc;

        void Awake()
        {
            if (damageable == null) damageable = GetComponent<Damageable>();
            _cc = GetComponent<CharacterController>();
            if (damageable != null)
            {
                damageable.Configure(kind == MobKind.MeleeBandit ? 70f : 45f, kind == MobKind.MeleeBandit ? 2f : 0f);
                damageable.Died += OnDied;
            }
        }

        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _player = player.transform;
        }

        void Update()
        {
            if (damageable != null && damageable.IsDead) return;
            if (_player == null) return;
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;

            Vector3 to = _player.position - transform.position;
            to.y = 0f;
            float dist = to.magnitude;
            if (dist > aggroRange) return;

            float range = kind == MobKind.RangedArcher ? 9f : attackRange;
            if (dist > range)
            {
                Vector3 dir = to.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
                Vector3 motion = dir * moveSpeed * Time.deltaTime;
                if (_cc != null) _cc.Move(motion + Vector3.down * 2f * Time.deltaTime);
                else transform.position += motion;
            }
            else if (_cooldown <= 0f)
            {
                Attack(to.normalized);
                _cooldown = attackCooldown;
            }
        }

        void Attack(Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            if (kind == MobKind.RangedArcher)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = transform.position + Vector3.up * 1.2f + dir;
                go.transform.localScale = Vector3.one * 0.2f;
                Object.Destroy(go.GetComponent<Collider>());
                var col = go.AddComponent<SphereCollider>();
                col.isTrigger = true;
                var rb = go.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.linearVelocity = dir * 16f;
                var proj = go.AddComponent<Projectile>();
                proj.Init(damage, 1.5f, gameObject, 0.5f);
                Object.Destroy(go, 3f);
            }
            else
            {
                var hits = Physics.OverlapSphere(transform.position + dir * 1.1f + Vector3.up, 1.1f);
                foreach (var hit in hits)
                {
                    if (hit.transform.IsChildOf(transform)) continue;
                    var dmg = hit.GetComponentInParent<Damageable>();
                    if (dmg != null && dmg.CompareTag("Player"))
                        dmg.ApplyDamage(damage, hit.transform.position, dir);
                }
            }
        }

        void OnDied()
        {
            if (lootMaterial != null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var inv = player.GetComponent<Inventory>();
                    if (inv != null) inv.Add(lootMaterial, lootCount);
                }
            }
            Destroy(gameObject, 0.4f);
        }
    }
}
