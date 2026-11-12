using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MedievalChinaARPG.World;

namespace MedievalChinaARPG.Editor
{
    /// <summary>
    /// Batchmode-friendly offline renders of hub/combat stylized look.
    /// Unity -batchmode -projectPath ... -executeMethod MedievalChinaARPG.Editor.CaptureStylizedScreens.CaptureAll -quit
    /// </summary>
    public static class CaptureStylizedScreens
    {
        public static void CaptureAll()
        {
            // Not under Temp/ — Unity deletes Temp on editor quit.
            var outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Captures"));
            Directory.CreateDirectory(outDir);
            CaptureOne("hub", false, outDir);
            CaptureOne("combat", true, outDir);
            Debug.Log("CaptureStylizedScreens wrote to " + outDir);
        }

        static void CaptureOne(string name, bool dusk, string outDir)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            VisualStyle.StyleDirectionalLight(light, dusk);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(7f, 1f, 7f);
            var gcol = dusk ? new Color(0.32f, 0.3f, 0.28f) : new Color(0.42f, 0.5f, 0.36f);
            VisualStyle.Apply(ground.GetComponent<Renderer>(),
                VisualStyle.MakeLit(gcol, Color.Lerp(gcol, new Color(0.15f, 0.12f, 0.1f), 0.45f), 0.35f));

            if (dusk) PropBuilder.BuildCombatArena();
            else PropBuilder.BuildHub();

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = dusk ? new Vector3(0f, 0.1f, -8f) : new Vector3(0f, 0.1f, -6f);
            VisualStyle.Apply(player.GetComponent<Renderer>(),
                VisualStyle.MakeLit(new Color(0.78f, 0.58f, 0.42f), new Color(0.35f, 0.22f, 0.2f), 1f));

            var camGo = new GameObject("CaptureCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = dusk ? new Color(0.72f, 0.42f, 0.38f) : new Color(0.55f, 0.72f, 0.88f);
            cam.farClipPlane = 120f;
            cam.transform.position = dusk ? new Vector3(0f, 5f, -14f) : new Vector3(0f, 4f, -12f);
            cam.transform.LookAt(dusk ? new Vector3(0f, 1f, 8f) : new Vector3(0f, 2f, 6f));

            VisualStyle.EnsureAtmosphere(gcol, dusk);

            int w = 1600, h = 900;
            var rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            cam.targetTexture = null;
            Object.DestroyImmediate(rt);

            var path = Path.Combine(outDir, "stylized-" + name + ".png");
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            Debug.Log("Wrote " + path);

            // Keep scene disposable
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
