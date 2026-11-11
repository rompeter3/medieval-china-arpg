using UnityEngine;

namespace MedievalChinaARPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        float _damage;
        float _knockback;
        GameObject _owner;
        float _radius = 0.6f;
        bool _spent;

        public void Init(float damage, float knockback, GameObject owner, float radius)
        {
            _damage = damage;
            _knockback = knockback;
            _owner = owner;
            _radius = radius;
        }

        void OnTriggerEnter(Collider other)
        {
            if (_spent) return;
            if (_owner != null && (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)))
                return;

            var dmg = other.GetComponentInParent<Damageable>();
            if (dmg == null) return;
            Vector3 dir = transform.forward;
            if (dmg.ApplyDamage(_damage, transform.position, dir))
            {
                _spent = true;
                Destroy(gameObject);
            }
        }

        void FixedUpdate()
        {
            if (_spent) return;
            var hits = Physics.OverlapSphere(transform.position, _radius * 0.5f);
            foreach (var hit in hits)
            {
                if (_owner != null && (hit.gameObject == _owner || hit.transform.IsChildOf(_owner.transform)))
                    continue;
                var dmg = hit.GetComponentInParent<Damageable>();
                if (dmg == null) continue;
                if (dmg.ApplyDamage(_damage, transform.position, transform.forward))
                {
                    _spent = true;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
