using System.Collections;
using UnityEngine;
using MedievalChinaARPG.Combat;

namespace MedievalChinaARPG.Player
{
    public class DodgeController : MonoBehaviour
    {
        [SerializeField] float dodgeDistance = 4.2f;
        [SerializeField] float dodgeDuration = 0.28f;
        [SerializeField] float iFrameDuration = 0.22f;
        [SerializeField] float cooldown = 0.55f;
        [SerializeField] float staminaCost = 18f;
        [SerializeField] Damageable damageable;
        [SerializeField] CharacterController characterController;

        float _cooldownLeft;
        public bool IsDodging { get; private set; }

        void Awake()
        {
            if (damageable == null) damageable = GetComponent<Damageable>();
            if (characterController == null) characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (_cooldownLeft > 0f) _cooldownLeft -= Time.deltaTime;
        }

        public bool TryDodge(Vector3 direction)
        {
            if (IsDodging || _cooldownLeft > 0f) return false;
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            direction.y = 0f;
            direction.Normalize();
            StartCoroutine(DodgeRoutine(direction));
            return true;
        }

        IEnumerator DodgeRoutine(Vector3 direction)
        {
            IsDodging = true;
            _cooldownLeft = cooldown;
            if (damageable != null) damageable.Invulnerable = true;

            float t = 0f;
            float iFrameLeft = iFrameDuration;
            while (t < dodgeDuration)
            {
                float dt = Time.deltaTime;
                t += dt;
                iFrameLeft -= dt;
                if (iFrameLeft <= 0f && damageable != null)
                    damageable.Invulnerable = false;

                if (characterController != null)
                    characterController.Move(direction * (dodgeDistance / dodgeDuration) * dt);
                else
                    transform.position += direction * (dodgeDistance / dodgeDuration) * dt;

                yield return null;
            }

            if (damageable != null) damageable.Invulnerable = false;
            IsDodging = false;
        }
    }
}
