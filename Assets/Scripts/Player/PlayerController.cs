using UnityEngine;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float walkSpeed = 4.2f;
        [SerializeField] float sprintSpeed = 7.2f;
        [SerializeField] float rotationSpeed = 14f;
        [SerializeField] float gravity = -20f;

        [Header("Refs")]
        [SerializeField] Transform cameraPivot;
        [SerializeField] CombatController combat;
        [SerializeField] DodgeController dodge;
        [SerializeField] Equipment equipment;

        CharacterController _cc;
        Vector3 _velocity;
        float _yaw;

        public bool InputLocked { get; set; }

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (combat == null) combat = GetComponent<CombatController>();
            if (dodge == null) dodge = GetComponent<DodgeController>();
            if (equipment == null) equipment = GetComponent<Equipment>();
            if (cameraPivot == null)
            {
                var cam = Camera.main;
                if (cam != null) cameraPivot = cam.transform;
            }
        }

        void Update()
        {
            if (InputLocked) return;

            HandleLook();
            HandleMove();
            HandleCombatInput();
        }

        void HandleLook()
        {
            float mx = Input.GetAxis("Mouse X");
            _yaw += mx * 120f * Time.deltaTime;
        }

        void HandleMove()
        {
            if (dodge != null && dodge.IsDodging) return;
            if (combat != null && combat.IsBusy)
            {
                ApplyGravityOnly();
                return;
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(h, 0f, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector3 camForward = cameraPivot != null ? cameraPivot.forward : transform.forward;
            Vector3 camRight = cameraPivot != null ? cameraPivot.right : transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camRight * input.x + camForward * input.z;
            bool sprint = Input.GetKey(KeyCode.LeftShift) && move.sqrMagnitude > 0.01f;
            float speed = sprint ? sprintSpeed : walkSpeed;

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            }

            Vector3 planar = move * speed;
            if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;
            _cc.Move((planar + Vector3.up * _velocity.y) * Time.deltaTime);
        }

        void ApplyGravityOnly()
        {
            if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;
            _cc.Move(Vector3.up * _velocity.y * Time.deltaTime);
        }

        void HandleCombatInput()
        {
            if (dodge != null && Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 dir = transform.forward;
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                if (Mathf.Abs(h) + Mathf.Abs(v) > 0.1f)
                {
                    Vector3 camForward = cameraPivot != null ? cameraPivot.forward : transform.forward;
                    Vector3 camRight = cameraPivot != null ? cameraPivot.right : transform.right;
                    camForward.y = 0f; camRight.y = 0f;
                    dir = (camRight * h + camForward * v).normalized;
                }
                dodge.TryDodge(dir);
                return;
            }

            if (combat == null) return;
            if (Input.GetButtonDown("Fire1")) combat.TryLightAttack();
            if (Input.GetButtonDown("Fire2")) combat.TryHeavyAttack();
            if (Input.GetKeyDown(KeyCode.Q)) combat.TryWeaponSkill();
        }
    }
}
