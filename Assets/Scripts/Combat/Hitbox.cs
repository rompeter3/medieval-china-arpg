using System.Collections.Generic;
using UnityEngine;

namespace MedievalChinaARPG.Combat
{
    public class Hitbox : MonoBehaviour
    {
        public LayerMask hitMask = ~0;
        readonly HashSet<Damageable> _hitThisSwing = new();

        public void BeginSwing() => _hitThisSwing.Clear();

        public void Pulse(Vector3 origin, float radius, float damage, float knockback, GameObject owner)
        {
            var cols = Physics.OverlapSphere(origin, radius, hitMask, QueryTriggerInteraction.Ignore);
            foreach (var col in cols)
            {
                if (col.gameObject == owner) continue;
                var dmg = col.GetComponentInParent<Damageable>();
                if (dmg == null || _hitThisSwing.Contains(dmg)) continue;
                if (owner != null)
                {
                    var ownerDmg = owner.GetComponent<Damageable>();
                    if (ownerDmg != null && ownerDmg == dmg) continue;
                }

                Vector3 dir = (col.transform.position - origin).normalized;
                if (dmg.ApplyDamage(damage, col.ClosestPoint(origin), dir))
                {
                    _hitThisSwing.Add(dmg);
                    var rb = col.attachedRigidbody;
                    if (rb != null && knockback > 0f)
                        rb.AddForce(dir * knockback + Vector3.up * (knockback * 0.25f), ForceMode.Impulse);
                }
            }
        }
    }
}
