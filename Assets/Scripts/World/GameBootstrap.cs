using UnityEngine;
using UnityEngine.SceneManagement;
using MedievalChinaARPG.Items;
using MedievalChinaARPG.Combat;
using MedievalChinaARPG.Player;
using MedievalChinaARPG.AI;

namespace MedievalChinaARPG.World
{
    /// <summary>
    /// Runtime scene builder so the slice is playable even without editor-authored prefabs.
    /// Scenes can be empty; this constructs hub / combat / character-create content on load.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] string mode = "Hub"; // Hub | Combat | CharacterCreate | Bootstrap
        [SerializeField] ItemDatabase database;

        static GameBootstrap _persist;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoAttach()
        {
            if (Object.FindFirstObjectByType<GameBootstrap>() != null) return;
            var go = new GameObject("GameBootstrap");
            go.AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            var scene = SceneManager.GetActiveScene().name;
            if (scene.Contains("Combat")) mode = "Combat";
            else if (scene.Contains("Character")) mode = "CharacterCreate";
            else if (scene.Contains("Hub")) mode = "Hub";
            else mode = "Bootstrap";

            if (mode == "Bootstrap")
            {
                DontDestroyOnLoad(gameObject);
                SceneLoader.LoadCharacterCreate();
                return;
            }

            EnsureDatabase();
            switch (mode)
            {
                case "CharacterCreate":
                    BuildCharacterCreate();
                    break;
                case "Hub":
                    BuildHub();
                    break;
                case "Combat":
                    BuildCombat();
                    break;
            }
        }

        void EnsureDatabase()
        {
            if (database == null)
                database = Resources.Load<ItemDatabase>("ItemDatabase");
            if (database == null)
                database = ItemCatalog.BuildRuntimeDatabase();
            ItemCatalog.Active = database;
            foreach (var w in database.weapons)
                WeaponMovesetFactory.EnsureMoveset(w);
        }

        void BuildCharacterCreate()
        {
            EnsureLightAndGround(new Color(0.55f, 0.62f, 0.48f), dusk: false);
            PropBuilder.BuildHub(); // backdrop for nicer preview
            var preview = CreatePlayerBody(new Vector3(0f, 0f, 0f), false);
            var custom = preview.AddComponent<CharacterCustomization>();
            var ui = gameObject.AddComponent<UI.CharacterCreateUI>();
            ui.Bind(custom, database);
            EnsureCamera(preview.transform, new Vector3(0f, 1.6f, -4f));
            VisualStyle.EnsureAtmosphere(new Color(0.55f, 0.62f, 0.48f), dusk: false);
        }

        void BuildHub()
        {
            EnsureLightAndGround(new Color(0.42f, 0.5f, 0.36f), dusk: false);
            PropBuilder.BuildHub();
            var player = CreatePlayerBody(new Vector3(0f, 0.1f, -6f), true);
            SetupPlayerSystems(player);
            EnsureCamera(player.transform, new Vector3(0f, 3f, -8f));
            CreatePortal(new Vector3(0f, 0.1f, 10f));
            VisualStyle.EnsureAtmosphere(new Color(0.42f, 0.5f, 0.36f), dusk: false);
            gameObject.AddComponent<UI.HUD>();
            gameObject.AddComponent<UI.InventoryUI>();
        }

        void BuildCombat()
        {
            EnsureLightAndGround(new Color(0.32f, 0.3f, 0.28f), dusk: true);
            PropBuilder.BuildCombatArena();
            var player = CreatePlayerBody(new Vector3(0f, 0.1f, -8f), true);
            SetupPlayerSystems(player);
            EnsureCamera(player.transform, new Vector3(0f, 4f, -10f));
            var spawner = gameObject.AddComponent<EnemySpawner>();
            var common = database.materials.Find(m => m != null && m.itemId == "mat_iron_scrap");
            var rare = database.materials.Find(m => m != null && m.itemId == "mat_spirit_jade");
            var drop = database.FindWeapon(WeaponCategory.Guandao);
            spawner.ConfigureLoot(common, rare, drop);
            spawner.SpawnDefaultEncounter();
            CreateReturnPortal(new Vector3(0f, 0.1f, -14f));
            VisualStyle.EnsureAtmosphere(new Color(0.32f, 0.3f, 0.28f), dusk: true);
            gameObject.AddComponent<UI.HUD>();
            gameObject.AddComponent<UI.InventoryUI>();
        }

        void CreateReturnPortal(Vector3 pos)
        {
            var gate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gate.name = "ReturnPortal";
            gate.transform.position = pos + Vector3.up;
            gate.transform.localScale = new Vector3(2.0f, 1.0f, 0.4f);
            VisualStyle.Apply(gate.GetComponent<Renderer>(),
                VisualStyle.MakeEmissive(new Color(0.65f, 0.45f, 0.25f), new Color(1.1f, 0.7f, 0.3f)));
            var trigger = gate.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.2f, 2f, 2f);
            gate.AddComponent<ReturnPortal>();
        }

        void SetupPlayerSystems(GameObject player)
        {
            player.tag = "Player";
            var inv = player.GetComponent<Inventory>() ?? player.AddComponent<Inventory>();
            var eq = player.GetComponent<Equipment>() ?? player.AddComponent<Equipment>();
            var upgrades = player.GetComponent<UpgradeSystem>() ?? player.AddComponent<UpgradeSystem>();
            upgrades.inventory = inv;
            upgrades.equipment = eq;
            player.AddComponent<Damageable>().Configure(120f, 0f);
            player.AddComponent<Hitbox>();
            player.AddComponent<CombatController>();
            player.AddComponent<DodgeController>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerStats>();
            var custom = player.AddComponent<CharacterCustomization>();
            if (PlayerSession.Appearance != null)
                custom.ApplyColors(PlayerSession.Appearance);

            var start = database.FindWeapon(PlayerSession.StartingWeaponId)
                        ?? database.FindWeapon(WeaponCategory.Jian)
                        ?? database.weapons.Find(w => w != null && w.isPlayable);
            if (start != null)
            {
                WeaponMovesetFactory.EnsureMoveset(start);
                eq.equippedWeapon = start;
                eq.weaponTier = 0;
            }

            // Starter kit
            var chest = database.armor.Find(a => a != null && a.slot == ArmorSlot.Chest);
            if (chest != null) inv.Add(chest, 1);
            var scrap = database.materials.Find(m => m != null && m.itemId == "mat_iron_scrap");
            if (scrap != null) inv.Add(scrap, 6);
            foreach (var w in database.weapons)
            {
                if (w != null && w.isPlayable && w != start)
                    inv.Add(w, 1);
            }
        }

        GameObject CreatePlayerBody(Vector3 pos, bool withController)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.transform.position = pos;
            var bodyRend = go.GetComponent<Renderer>();
            VisualStyle.Apply(bodyRend, VisualStyle.MakeLit(new Color(0.78f, 0.58f, 0.42f),
                new Color(0.35f, 0.22f, 0.2f), 1f));

            // Simple robe shell
            var robe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            robe.name = "Robe";
            robe.transform.SetParent(go.transform, false);
            robe.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            robe.transform.localScale = new Vector3(0.85f, 0.9f, 0.55f);
            var robeRend = robe.GetComponent<Renderer>();
            VisualStyle.Apply(robeRend, VisualStyle.MakeLit(new Color(0.45f, 0.12f, 0.1f),
                new Color(0.18f, 0.08f, 0.1f), 0.9f));
            Object.Destroy(robe.GetComponent<Collider>());

            if (withController)
            {
                Object.Destroy(go.GetComponent<CapsuleCollider>());
                var cc = go.AddComponent<CharacterController>();
                cc.height = 1.8f;
                cc.radius = 0.35f;
                cc.center = Vector3.up * 0.9f;
            }
            var hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.name = "Hair";
            hair.transform.SetParent(go.transform);
            hair.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            hair.transform.localScale = new Vector3(0.55f, 0.35f, 0.55f);
            var hairRend = hair.GetComponent<Renderer>();
            VisualStyle.Apply(hairRend, VisualStyle.MakeLit(new Color(0.1f, 0.08f, 0.07f),
                new Color(0.04f, 0.03f, 0.03f), 0.5f));
            Object.Destroy(hair.GetComponent<Collider>());

            // Topknot
            var knot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            knot.name = "Topknot";
            knot.transform.SetParent(hair.transform, false);
            knot.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            knot.transform.localScale = new Vector3(0.45f, 0.55f, 0.45f);
            VisualStyle.Apply(knot.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.08f, 0.06f, 0.05f)));
            Object.Destroy(knot.GetComponent<Collider>());
            return go;
        }

        void CreatePortal(Vector3 pos)
        {
            var gate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gate.name = "Portal";
            gate.transform.position = pos + Vector3.up;
            gate.transform.localScale = new Vector3(2.2f, 1.2f, 0.4f);
            VisualStyle.Apply(gate.GetComponent<Renderer>(),
                VisualStyle.MakeEmissive(new Color(0.3f, 0.55f, 0.8f), new Color(0.4f, 0.85f, 1.4f)));
            var trigger = gate.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.2f, 2f, 2f);
            gate.AddComponent<HubPortal>();
        }

        void EnsureCamera(Transform target, Vector3 localOffset)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                cam.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            var tp = cam.GetComponent<ThirdPersonCamera>() ?? cam.gameObject.AddComponent<ThirdPersonCamera>();
            tp.SetTarget(target);
            cam.transform.position = target.position + localOffset;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void EnsureLightAndGround(Color groundColor, bool dusk)
        {
            var existing = Object.FindFirstObjectByType<Light>();
            if (existing == null)
            {
                var lightGo = new GameObject("Directional Light");
                existing = lightGo.AddComponent<Light>();
                existing.type = LightType.Directional;
            }
            VisualStyle.StyleDirectionalLight(existing, dusk);

            if (GameObject.Find("Ground") == null)
            {
                var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.localScale = new Vector3(7f, 1f, 7f);
                VisualStyle.Apply(ground.GetComponent<Renderer>(),
                    VisualStyle.MakeLit(groundColor, Color.Lerp(groundColor, new Color(0.15f, 0.12f, 0.1f), 0.45f), 0.35f));
            }
        }
    }
}
