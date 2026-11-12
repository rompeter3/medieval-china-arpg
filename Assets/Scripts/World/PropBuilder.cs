using UnityEngine;

namespace MedievalChinaARPG.World
{
    public static class PropBuilder
    {
        public static void BuildHub()
        {
            BuildPaifang(new Vector3(0f, 0f, 9f));
            BuildStall(new Vector3(-6.2f, 0f, 0.5f), new Color(0.62f, 0.22f, 0.16f), new Color(0.5f, 0.4f, 0.28f));
            BuildStall(new Vector3(6.2f, 0f, 2.2f), new Color(0.18f, 0.38f, 0.34f), new Color(0.48f, 0.38f, 0.26f));
            BuildTeaHouse(new Vector3(-9f, 0f, -4f));
            BuildStonePath();

            CreateLantern(new Vector3(-2.2f, 0f, 4.5f));
            CreateLantern(new Vector3(2.2f, 0f, 4.5f));
            CreateLantern(new Vector3(-5.5f, 0f, -3f));
            CreateLantern(new Vector3(5.5f, 0f, -2f));
            CreateLantern(new Vector3(0f, 0f, -8f));

            for (int i = 0; i < 12; i++)
            {
                float x = -14f + i * 2.4f;
                CreateBambooCluster(new Vector3(x, 0f, 15f));
                CreateBambooCluster(new Vector3(x + 0.4f, 0f, -13f));
            }

            CreateWaterPond(new Vector3(10f, 0.02f, -7f), new Vector3(6f, 1f, 4f));
            CreateRock(new Vector3(-11f, 0f, 3f), 1.2f);
            CreateRock(new Vector3(11f, 0f, 6f), 1.5f);
        }

        public static void BuildCombatArena()
        {
            for (int i = 0; i < 18; i++)
            {
                float ang = i / 18f * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * 19f;
                CreateCliff(p + Vector3.up * 2.2f, 1f + (i % 3) * 0.25f);
            }

            for (int i = 0; i < 24; i++)
            {
                float ang = i / 24f * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * 14.5f;
                CreateBambooCluster(p);
            }

            CreateBox("ArenaStone", new Vector3(0f, 0.04f, 8f), new Vector3(12f, 0.08f, 16f),
                new Color(0.42f, 0.4f, 0.36f), new Color(0.22f, 0.2f, 0.2f));
            CreateLantern(new Vector3(-5.5f, 0f, 2f));
            CreateLantern(new Vector3(5.5f, 0f, 2f));
            CreateLantern(new Vector3(0f, 0f, 18f));
            CreateLantern(new Vector3(-8f, 0f, 12f));
            CreateLantern(new Vector3(8f, 0f, 12f));
            CreateRock(new Vector3(-7f, 0f, 10f), 1.4f);
            CreateRock(new Vector3(7.5f, 0f, 6f), 1.1f);
            CreateWaterPond(new Vector3(0f, 0.02f, 20.5f), new Vector3(8f, 1f, 3f));
        }

        static void BuildPaifang(Vector3 origin)
        {
            CreateBox("GateLeft", origin + new Vector3(-3.3f, 1.7f, 0f), new Vector3(0.7f, 3.4f, 0.7f),
                new Color(0.32f, 0.22f, 0.16f));
            CreateBox("GateRight", origin + new Vector3(3.3f, 1.7f, 0f), new Vector3(0.7f, 3.4f, 0.7f),
                new Color(0.32f, 0.22f, 0.16f));
            CreateBox("GateBeam", origin + new Vector3(0f, 3.5f, 0f), new Vector3(7.4f, 0.45f, 0.9f),
                new Color(0.62f, 0.16f, 0.14f));
            CreateCurvedRoof("GateRoof", origin + new Vector3(0f, 4.15f, 0f), new Vector3(8.2f, 0.35f, 2.2f),
                new Color(0.45f, 0.1f, 0.08f));
            CreateBox("GateSign", origin + new Vector3(0f, 2.7f, -0.35f), new Vector3(2.2f, 0.7f, 0.12f),
                new Color(0.85f, 0.72f, 0.35f));
        }

        static void BuildStall(Vector3 origin, Color roof, Color wood)
        {
            CreateBox("StallBody", origin + new Vector3(0f, 0.8f, 0f), new Vector3(2.6f, 1.6f, 1.7f), wood);
            CreateCurvedRoof("StallRoof", origin + new Vector3(0f, 1.85f, 0f), new Vector3(3.1f, 0.22f, 2.2f), roof);
            CreateBox("StallCounter", origin + new Vector3(0f, 0.55f, -0.95f), new Vector3(2.4f, 0.2f, 0.4f),
                new Color(0.4f, 0.28f, 0.18f));
        }

        static void BuildTeaHouse(Vector3 origin)
        {
            CreateBox("TeaHouse", origin + new Vector3(0f, 1.2f, 0f), new Vector3(4.2f, 2.4f, 3.2f),
                new Color(0.55f, 0.42f, 0.3f));
            CreateCurvedRoof("TeaRoofLower", origin + new Vector3(0f, 2.55f, 0f), new Vector3(5f, 0.28f, 4f),
                new Color(0.55f, 0.14f, 0.12f));
            CreateCurvedRoof("TeaRoofUpper", origin + new Vector3(0f, 3.35f, 0f), new Vector3(3.4f, 0.25f, 2.6f),
                new Color(0.48f, 0.12f, 0.1f));
            CreateBox("TeaDoor", origin + new Vector3(0f, 0.9f, -1.65f), new Vector3(1.1f, 1.8f, 0.08f),
                new Color(0.25f, 0.12f, 0.1f));
        }

        static void BuildStonePath()
        {
            for (int i = 0; i < 8; i++)
            {
                CreateBox($"Path_{i}", new Vector3(0f, 0.03f, -6f + i * 2f), new Vector3(2.2f, 0.06f, 1.6f),
                    new Color(0.48f, 0.46f, 0.42f), new Color(0.28f, 0.26f, 0.24f));
            }
        }

        static void CreateCurvedRoof(string name, Vector3 pos, Vector3 scale, Color color)
        {
            CreateBox(name, pos, scale, color, Color.Lerp(color, Color.black, 0.35f));
            // Upturned eaves as small angled tips
            var left = GameObject.CreatePrimitive(PrimitiveType.Cube);
            left.name = name + "_EaveL";
            left.transform.position = pos + new Vector3(-scale.x * 0.48f, 0.12f, 0f);
            left.transform.localScale = new Vector3(0.55f, 0.18f, scale.z * 0.9f);
            left.transform.rotation = Quaternion.Euler(0f, 0f, 22f);
            VisualStyle.Apply(left.GetComponent<Renderer>(), VisualStyle.MakeLit(color));
            Object.Destroy(left.GetComponent<Collider>());

            var right = GameObject.CreatePrimitive(PrimitiveType.Cube);
            right.name = name + "_EaveR";
            right.transform.position = pos + new Vector3(scale.x * 0.48f, 0.12f, 0f);
            right.transform.localScale = new Vector3(0.55f, 0.18f, scale.z * 0.9f);
            right.transform.rotation = Quaternion.Euler(0f, 0f, -22f);
            VisualStyle.Apply(right.GetComponent<Renderer>(), VisualStyle.MakeLit(color));
            Object.Destroy(right.GetComponent<Collider>());
        }

        static void CreateCliff(Vector3 pos, float scaleMul)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Cliff";
            go.transform.position = pos;
            go.transform.localScale = new Vector3(2.4f, 4.4f, 2.4f) * scaleMul;
            go.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            VisualStyle.Apply(go.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.4f, 0.38f, 0.34f), new Color(0.2f, 0.18f, 0.18f), 0.4f));
        }

        static void CreateRock(Vector3 pos, float scale)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Rock";
            go.transform.position = pos + Vector3.up * (0.35f * scale);
            go.transform.localScale = new Vector3(1.1f, 0.7f, 0.9f) * scale;
            VisualStyle.Apply(go.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.45f, 0.43f, 0.4f), new Color(0.22f, 0.2f, 0.2f), 0.35f));
        }

        static void CreateWaterPond(Vector3 pos, Vector3 scale)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Water";
            go.transform.position = pos;
            go.transform.localScale = new Vector3(scale.x, 0.05f, scale.z);
            Object.Destroy(go.GetComponent<Collider>());
            VisualStyle.Apply(go.GetComponent<Renderer>(), VisualStyle.MakeWater());
        }

        static void CreateBox(string name, Vector3 pos, Vector3 scale, Color color, Color? shade = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = scale;
            VisualStyle.Apply(go.GetComponent<Renderer>(), VisualStyle.MakeLit(color, shade));
        }

        static void CreateBambooCluster(Vector3 pos)
        {
            int count = 3 + Random.Range(0, 2);
            for (int i = 0; i < count; i++)
            {
                var offset = new Vector3(Random.Range(-0.25f, 0.25f), 0f, Random.Range(-0.25f, 0.25f));
                CreateBamboo(pos + offset, 1f + Random.Range(-0.15f, 0.25f));
            }
        }

        static void CreateBamboo(Vector3 pos, float heightMul = 1f)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Bamboo";
            float h = 1.7f * heightMul;
            go.transform.position = pos + Vector3.up * h;
            go.transform.localScale = new Vector3(0.14f, h, 0.14f);
            VisualStyle.Apply(go.GetComponent<Renderer>(),
                VisualStyle.MakeFoliage(new Color(0.22f, 0.42f, 0.22f), new Color(0.45f, 0.65f, 0.3f)));
            Object.Destroy(go.GetComponent<Collider>());

            // Leaf crown
            var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.name = "BambooLeaves";
            leaves.transform.SetParent(go.transform, false);
            leaves.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            leaves.transform.localScale = new Vector3(3.2f, 1.4f, 3.2f);
            VisualStyle.Apply(leaves.GetComponent<Renderer>(),
                VisualStyle.MakeFoliage(new Color(0.25f, 0.48f, 0.24f), new Color(0.6f, 0.8f, 0.35f)));
            Object.Destroy(leaves.GetComponent<Collider>());
        }

        static void CreateLantern(Vector3 pos)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "LanternPole";
            pole.transform.position = pos + Vector3.up * 1.3f;
            pole.transform.localScale = new Vector3(0.09f, 1.3f, 0.09f);
            VisualStyle.Apply(pole.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.22f, 0.15f, 0.1f)));

            var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "Lantern";
            lamp.transform.position = pos + Vector3.up * 2.7f;
            lamp.transform.localScale = new Vector3(0.5f, 0.65f, 0.5f);
            VisualStyle.Apply(lamp.GetComponent<Renderer>(),
                VisualStyle.MakeEmissive(new Color(0.95f, 0.55f, 0.2f), new Color(1.6f, 0.75f, 0.25f)));
            Object.Destroy(lamp.GetComponent<Collider>());

            var hat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hat.name = "LanternHat";
            hat.transform.SetParent(lamp.transform, false);
            hat.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            hat.transform.localScale = new Vector3(1.25f, 0.15f, 1.25f);
            VisualStyle.Apply(hat.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.35f, 0.12f, 0.1f)));
            Object.Destroy(hat.GetComponent<Collider>());

            var lightGo = new GameObject("LanternLight");
            lightGo.transform.SetParent(lamp.transform);
            lightGo.transform.localPosition = Vector3.zero;
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.65f, 0.3f);
            light.range = 8f;
            light.intensity = 2.2f;
            // Soft point shadows from many lanterns flood the atlas and spam Console.
            light.shadows = LightShadows.None;
        }
    }
}
