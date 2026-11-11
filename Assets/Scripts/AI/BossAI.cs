using UnityEngine;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.AI
{
    public enum BossPhase
    {
        Sweep,
        Lunge,
        Storm
    }

    public class BossAI : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 2.6f;
        [SerializeField] float telegraphTime = 0.85f;
        [SerializeField] Damageable damageable;
        [SerializeField] MaterialItem rareLoot;
        [SerializeField] WeaponDefinition weaponDrop;
        [SerializeField] LineRenderer telegraphLine;

        Transform _player;
        BossPhase _phase = BossPhase.Sweep;
        float _timer;
        bool _winding;
        bool _executed;
        CharacterController _cc;
        Color _telegraphColor = new Color(1f, 0.35f, 0.1f, 0.85f);

        public BossPhase Phase => _phase;
        public bool IsTelegraphing => _winding;

        void Awake()
        {
            if (damageable == null) damageable = GetComponent<Damageable>();
            _cc = GetComponent<CharacterController>();
            if (damageable != null)
            {
                damageable.Configure(420f, 8f);
                damageable.Died += OnDied;
                damageable.HealthChanged += OnHealthChanged;
            }

            if (telegraphLine == null)
            {
                telegraphLine = gameObject.AddComponent<LineRenderer>();
                telegraphLine.positionCount = 2;
                telegraphLine.startWidth = 0.15f;
                telegraphLine.endWidth = 0.05f;
                telegraphLine.material = new Material(Shader.Find("Sprites/Default"));
                telegraphLine.startColor = _telegraphColor;
                telegraphLine.endColor = _telegraphColor;
                telegraphLine.enabled = false;
            }
        }

        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _player = player.transform;
            BeginTelegraph();
        }

        void Update()
        {
            if (damageable != null && damageable.IsDead) return;
            if (_player == null) return;

            Vector3 to = _player.position - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), 5f * Time.deltaTime);

            if (!_winding)
            {
                float desired = _phase == BossPhase.Lunge ? 2.2f : 4.5f;
                if (to.magnitude > desired)
                {
                    Vector3 motion = to.normalized * moveSpeed * Time.deltaTime;
                    if (_cc != null) _cc.Move(motion + Vector3.down * Time.deltaTime);
                    else transform.position += motion;
                }
                return;
            }

            _timer -= Time.deltaTime;
            UpdateTelegraphVisual(to.normalized);
            if (_timer <= 0f && !_executed)
            {
                _executed = true;
                ExecutePhase(to.normalized);
                telegraphLine.enabled = false;
                _winding = false;
                Invoke(nameof(BeginTelegraph), 1.1f);
            }
        }

        void BeginTelegraph()
        {
            _winding = true;
            _executed = false;
            _timer = telegraphTime;
            telegraphLine.enabled = true;
        }

        void UpdateTelegraphVisual(Vector3 dir)
        {
            Vector3 origin = transform.position + Vector3.up * 0.2f;
            float length = _phase == BossPhase.Storm ? 7f : (_phase == BossPhase.Lunge ? 5f : 4f);
            telegraphLine.SetPosition(0, origin);
            telegraphLine.SetPosition(1, origin + dir * length);
        }

        void ExecutePhase(Vector3 dir)
        {
            switch (_phase)
            {
                case BossPhase.Sweep:
                    Pulse(transform.position + Vector3.up, 3.2f, 22f, 5f);
                    break;
                case BossPhase.Lunge:
                    if (_cc != null) _cc.Move(dir * 4.5f);
                    else transform.position += dir * 4.5f;
                    Pulse(transform.position + dir + Vector3.up, 1.8f, 28f, 7f);
                    break;
                case BossPhase.Storm:
                    for (int i = 0; i < 6; i++)
                    {
                        float ang = i * 60f * Mathf.Deg2Rad;
                        Vector3 d = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
                        Pulse(transform.position + d * 2.5f + Vector3.up, 1.4f, 16f, 3f);
                    }
                    break;
            }
        }

        void Pulse(Vector3 origin, float radius, float dmg, float knock)
        {
            var hits = Physics.OverlapSphere(origin, radius);
            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;
                var d = hit.GetComponentInParent<Damageable>();
                if (d == null) continue;
                if (d.CompareTag("Player"))
                    d.ApplyDamage(dmg, hit.transform.position, (hit.transform.position - transform.position).normalized);
            }
        }

        void OnHealthChanged(float current, float max)
        {
            float pct = current / max;
            if (pct <= 0.35f) _phase = BossPhase.Storm;
            else if (pct <= 0.7f) _phase = BossPhase.Lunge;
            else _phase = BossPhase.Sweep;
        }

        void OnDied()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var inv = player.GetComponent<Inventory>();
                if (inv != null)
                {
                    if (rareLoot != null) inv.Add(rareLoot, 5);
                    if (weaponDrop != null) inv.Add(weaponDrop, 1);
                }
            }
            Destroy(gameObject, 1.2f);
        }
    }
}
