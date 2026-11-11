using UnityEngine;

namespace MedievalChinaARPG.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 2.2f, -5.5f);
        [SerializeField] float sensitivity = 140f;
        [SerializeField] float minPitch = -25f;
        [SerializeField] float maxPitch = 55f;
        [SerializeField] float followLerp = 12f;

        float _yaw;
        float _pitch = 12f;

        public void SetTarget(Transform t) => target = t;

        void LateUpdate()
        {
            if (target == null) return;
            _yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            _pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 desired = target.position + rot * offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1.4f - transform.position, Vector3.up);
        }
    }
}
