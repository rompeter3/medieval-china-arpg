using UnityEngine;
using MedievalChinaARPG.Items;

namespace MedievalChinaARPG.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] MaterialItem commonLoot;
        [SerializeField] MaterialItem rareLoot;
        [SerializeField] WeaponDefinition bossWeaponDrop;

        public void SpawnDefaultEncounter()
        {
            SpawnMob(new Vector3(-4f, 0f, 6f), MobKind.MeleeBandit, new Color(0.45f, 0.25f, 0.2f));
            SpawnMob(new Vector3(4f, 0f, 7f), MobKind.MeleeBandit, new Color(0.4f, 0.22f, 0.18f));
            SpawnMob(new Vector3(0f, 0f, 10f), MobKind.RangedArcher, new Color(0.25f, 0.3f, 0.35f));
            SpawnBoss(new Vector3(0f, 0f, 16f));
        }

        void SpawnMob(Vector3 pos, MobKind kind, Color color)
        {
            var go = CreateCapsule(kind == MobKind.MeleeBandit ? "MeleeBandit" : "RangedArcher", pos, color, 1.7f);
            go.tag = "Enemy";
            var ai = go.AddComponent<MobAI>();
            var so = new SerializedMob(ai, kind, commonLoot);
            so.Apply();
        }

        void SpawnBoss(Vector3 pos)
        {
            var go = CreateCapsule("WarlordBoss", pos, new Color(0.55f, 0.1f, 0.08f), 2.4f);
            go.tag = "Enemy";
            go.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            var boss = go.AddComponent<BossAI>();
            var fieldLoot = typeof(BossAI).GetField("rareLoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fieldWeapon = typeof(BossAI).GetField("weaponDrop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldLoot?.SetValue(boss, rareLoot);
            fieldWeapon?.SetValue(boss, bossWeaponDrop);
        }

        GameObject CreateCapsule(string name, Vector3 pos, Color color, float height)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.transform.position = pos;
            go.GetComponent<Renderer>().material.color = color;
            var cc = go.AddComponent<CharacterController>();
            cc.height = height;
            cc.center = Vector3.up * (height * 0.15f);
            go.AddComponent<Combat.Damageable>();
            return go;
        }

        // Helper to set private SerializeFields without requiring editor wiring for runtime-built enemies.
        class SerializedMob
        {
            readonly MobAI _ai;
            readonly MobKind _kind;
            readonly MaterialItem _loot;
            public SerializedMob(MobAI ai, MobKind kind, MaterialItem loot)
            {
                _ai = ai; _kind = kind; _loot = loot;
            }
            public void Apply()
            {
                var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                typeof(MobAI).GetField("kind", flags)?.SetValue(_ai, _kind);
                typeof(MobAI).GetField("lootMaterial", flags)?.SetValue(_ai, _loot);
                typeof(MobAI).GetField("lootCount", flags)?.SetValue(_ai, _kind == MobKind.MeleeBandit ? 2 : 3);
                typeof(MobAI).GetField("damage", flags)?.SetValue(_ai, _kind == MobKind.MeleeBandit ? 12f : 9f);
                typeof(MobAI).GetField("attackRange", flags)?.SetValue(_ai, _kind == MobKind.MeleeBandit ? 1.6f : 9f);
            }
        }

        public void ConfigureLoot(MaterialItem common, MaterialItem rare, WeaponDefinition bossWeapon)
        {
            commonLoot = common;
            rareLoot = rare;
            bossWeaponDrop = bossWeapon;
        }
    }
}
