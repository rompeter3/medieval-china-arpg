using UnityEngine;
using UnityEngine.Rendering;

namespace MedievalChinaARPG.World
{
    /// <summary>
    /// Shared stylized look helpers: materials, fog/sky, lighting mood.
    /// </summary>
    public static class VisualStyle
    {
        static Material MakeUrpLit(Color color)
        {
            var lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Universal Render Pipeline/Simple Lit") ?? Shader.Find("Standard");
            var fallback = new Material(lit);
            if (fallback.HasProperty("_BaseColor"))
                fallback.SetColor("_BaseColor", color);
            else
                fallback.color = color;
            return fallback;
        }

        static bool Usable(Shader shader) => shader != null && shader.isSupported;

        public static Material MakeLit(Color color, Color? shade = null, float rim = 0.85f, bool leaf = false)
        {
            var shader = Shader.Find("MedievalChinaARPG/StylizedRim");
            if (!Usable(shader))
                return MakeUrpLit(color);

            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_ShadeColor", shade ?? Color.Lerp(color, new Color(0.15f, 0.12f, 0.18f), 0.55f));
            mat.SetColor("_RimColor", new Color(1f, 0.9f, 0.7f, 1f));
            mat.SetFloat("_RimPower", 2.6f);
            mat.SetFloat("_RimIntensity", rim);
            mat.SetFloat("_Bands", 3f);
            mat.SetFloat("_LeafSway", leaf ? 1f : 0f);
            mat.SetFloat("_SwayStrength", leaf ? 0.1f : 0f);
            return mat;
        }

        public static Material MakeFoliage(Color baseCol, Color tipCol)
        {
            var shader = Shader.Find("MedievalChinaARPG/StylizedFoliage");
            if (!Usable(shader)) return MakeLit(baseCol, leaf: true);
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", baseCol);
            mat.SetColor("_TipColor", tipCol);
            mat.SetColor("_RimColor", new Color(0.8f, 1f, 0.55f, 1f));
            mat.SetFloat("_SwayStrength", 0.14f);
            mat.SetFloat("_Translucency", 0.5f);
            return mat;
        }

        public static Material MakeEmissive(Color color, Color emission)
        {
            var shader = Shader.Find("MedievalChinaARPG/StylizedEmissive");
            if (!Usable(shader)) return MakeLit(color, rim: 1.2f);
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_Emission", emission);
            mat.SetFloat("_Pulse", 0.22f);
            return mat;
        }

        public static Material MakeWater()
        {
            var shader = Shader.Find("MedievalChinaARPG/StylizedWater");
            if (!Usable(shader)) return MakeLit(new Color(0.2f, 0.45f, 0.5f));
            return new Material(shader);
        }

        public static void Apply(Renderer renderer, Material mat)
        {
            if (renderer != null && mat != null)
                renderer.sharedMaterial = mat;
        }

        public static void EnsureAtmosphere(Color groundHint, bool dusk)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = dusk ? 0.008f : 0.005f;
            RenderSettings.fogColor = dusk
                ? new Color(0.55f, 0.35f, 0.38f)
                : new Color(0.62f, 0.72f, 0.78f);

            RenderSettings.ambientMode = AmbientMode.Trilight;
            if (dusk)
            {
                RenderSettings.ambientSkyColor = new Color(0.75f, 0.45f, 0.4f);
                RenderSettings.ambientEquatorColor = new Color(0.45f, 0.28f, 0.3f);
                RenderSettings.ambientGroundColor = new Color(0.18f, 0.12f, 0.1f);
            }
            else
            {
                RenderSettings.ambientSkyColor = new Color(0.55f, 0.7f, 0.85f);
                RenderSettings.ambientEquatorColor = new Color(0.55f, 0.58f, 0.5f);
                RenderSettings.ambientGroundColor = Color.Lerp(groundHint, new Color(0.2f, 0.18f, 0.15f), 0.4f);
            }

            if (Camera.main != null)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = dusk
                    ? new Color(0.72f, 0.42f, 0.38f)
                    : new Color(0.55f, 0.72f, 0.88f);
                Camera.main.farClipPlane = 120f;
            }

            // Soft vignette-like feel via slightly lifted exposure on additional fill light.
            EnsureFillLight(dusk);
        }

        static void EnsureFillLight(bool dusk)
        {
            if (GameObject.Find("StylizedFillLight") != null) return;
            var go = new GameObject("StylizedFillLight");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.None;
            light.intensity = dusk ? 0.35f : 0.25f;
            light.color = dusk ? new Color(0.55f, 0.45f, 0.7f) : new Color(0.55f, 0.7f, 0.95f);
            go.transform.rotation = Quaternion.Euler(15f, 140f, 0f);
        }

        public static void StyleDirectionalLight(Light light, bool dusk)
        {
            if (light == null) return;
            light.shadows = LightShadows.Soft;
            if (dusk)
            {
                light.color = new Color(1f, 0.62f, 0.42f);
                light.intensity = 1.35f;
                light.transform.rotation = Quaternion.Euler(28f, -40f, 0f);
            }
            else
            {
                light.color = new Color(1f, 0.95f, 0.85f);
                light.intensity = 1.2f;
                light.transform.rotation = Quaternion.Euler(42f, -25f, 0f);
            }
        }
    }
}
