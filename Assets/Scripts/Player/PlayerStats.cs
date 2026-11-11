using UnityEngine;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] Damageable damageable;
        [SerializeField] Equipment equipment;

        void Awake()
        {
            if (damageable == null) damageable = GetComponent<Damageable>();
            if (equipment == null) equipment = GetComponent<Equipment>();
        }

        void OnEnable()
        {
            if (equipment != null) equipment.Changed += RefreshArmor;
            RefreshArmor();
        }

        void OnDisable()
        {
            if (equipment != null) equipment.Changed -= RefreshArmor;
        }

        void RefreshArmor()
        {
            if (damageable != null && equipment != null)
                damageable.SetArmor(equipment.TotalArmor);
        }
    }
}
