using UnityEngine;

namespace MedievalChinaARPG.World
{
    public static class PropBuilder
    {
        public static void BuildHub()
        {
            // Gate
            CreateBox("GateLeft", new Vector3(-3.2f, 1.5f, 8f), new Vector3(0.6f, 3f, 0.6f), new Color(0.35f, 0.28f, 0.22f));
            CreateBox("GateRight", new Vector3(3.2f, 1.5f, 8f), new Vector3(0.6f, 3f, 0.6f), new Color(0.35f, 0.28f, 0.22f));
            CreateBox("GateBeam", new Vector3(0f, 3.2f, 8f), new Vector3(7f, 0.5f, 0.8f), new Color(0.55f, 0.18f, 0.14f));
            CreateBox("GateRoof", new Vector3(0f, 3.8f, 8f), new Vector3(7.6f, 0.35f, 1.6f), new Color(0.45f, 0.12f, 0.1f));

            // Stalls
            CreateBox("StallA", new Vector3(-6f, 0.75f, 0f), new Vector3(2.4f, 1.5f, 1.6f), new Color(0.5f, 0.4f, 0.28f));
            CreateBox("StallARoof", new Vector3(-6f, 1.7f, 0f), new Vector3(2.8f, 0.2f, 2f), new Color(0.6f, 0.2f, 0.15f));
            CreateBox("StallB", new Vector3(6f, 0.75f, 2f), new Vector3(2.4f, 1.5f, 1.6f), new Color(0.48f, 0.38f, 0.26f));
            CreateBox("StallBRoof", new Vector3(6f, 1.7f, 2f), new Vector3(2.8f, 0.2f, 2f), new Color(0.2f, 0.35f, 0.3f));

            // Lanterns
            CreateLantern(new Vector3(-2f, 0f, 4f));
            CreateLantern(new Vector3(2f, 0f, 4f));
            CreateLantern(new Vector3(-5f, 0f, -3f));
            CreateLantern(new Vector3(5f, 0f, -2f));

            // Bamboo / pine placeholders
            for (int i = 0; i < 10; i++)
            {
                float x = -12f + i * 2.4f;
                CreateBamboo(new Vector3(x, 0f, 14f));
                CreateBamboo(new Vector3(x, 0f, -12f));
            }
        }

        public static void BuildCombatArena()
        {
            // Arena ring
            for (int i = 0; i < 16; i++)
            {
                float ang = i / 16f * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * 18f;
                CreateBox($"Cliff_{i}", p + Vector3.up * 2f, new Vector3(2.2f, 4f, 2.2f), new Color(0.4f, 0.38f, 0.34f));
            }

            for (int i = 0; i < 20; i++)
            {
                float ang = i / 20f * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * 14f;
                CreateBamboo(p);
            }

            CreateBox("ArenaStone", new Vector3(0f, 0.05f, 8f), new Vector3(10f, 0.1f, 14f), new Color(0.42f, 0.4f, 0.36f));
            CreateLantern(new Vector3(-5f, 0f, 2f));
            CreateLantern(new Vector3(5f, 0f, 2f));
            CreateLantern(new Vector3(0f, 0f, 18f));
        }

        static void CreateBox(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = scale;
            ApplyStylized(go, color);
        }

        static void CreateBamboo(Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Bamboo";
            go.transform.position = pos + Vector3.up * 1.6f;
            go.transform.localScale = new Vector3(0.18f, 1.6f, 0.18f);
            ApplyStylized(go, new Color(0.28f, 0.48f, 0.25f), true);
        }

        static void CreateLantern(Vector3 pos)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "LanternPole";
            pole.transform.position = pos + Vector3.up * 1.2f;
            pole.transform.localScale = new Vector3(0.1f, 1.2f, 0.1f);
            ApplyStylized(pole, new Color(0.25f, 0.18f, 0.12f));

            var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "Lantern";
            lamp.transform.position = pos + Vector3.up * 2.5f;
            lamp.transform.localScale = new Vector3(0.45f, 0.55f, 0.45f);
            ApplyStylized(lamp, new Color(0.95f, 0.55f, 0.2f));

            var lightGo = new GameObject("LanternLight");
            lightGo.transform.SetParent(lamp.transform);
            lightGo.transform.localPosition = Vector3.zero;
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.7f, 0.35f);
            light.range = 6f;
            light.intensity = 1.4f;
        }

        static void ApplyStylized(GameObject go, Color color, bool leaf = false)
        {
            var renderer = go.GetComponent<Renderer>();
            var shader = Shader.Find("MedievalChinaARPG/StylizedRim");
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.SetColor("_BaseColor", color);
                mat.SetFloat("_RimPower", 3.2f);
                mat.SetColor("_RimColor", new Color(0.85f, 0.9f, 1f, 1f));
                mat.SetFloat("_LeafSway", leaf ? 1f : 0f);
                renderer.material = mat;
            }
            else
            {
                renderer.material.color = color;
            }
        }
    }
}
