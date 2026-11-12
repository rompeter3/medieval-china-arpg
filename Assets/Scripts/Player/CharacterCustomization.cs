using UnityEngine;

namespace MedievalChinaARPG.Player
{
    [System.Serializable]
    public class AppearancePreset
    {
        public string presetName;
        public Color skinColor = new Color(0.92f, 0.78f, 0.66f);
        public Color hairColor = new Color(0.12f, 0.1f, 0.08f);
        public Color outfitPrimary = new Color(0.45f, 0.12f, 0.1f);
        public Color outfitSecondary = new Color(0.15f, 0.22f, 0.28f);
        public int faceIndex;
        public int hairIndex;
    }

    public class CharacterCustomization : MonoBehaviour
    {
        [SerializeField] Renderer bodyRenderer;
        [SerializeField] Renderer hairRenderer;
        [SerializeField] AppearancePreset[] presets;

        public AppearancePreset Current { get; private set; }
        public int PresetIndex { get; private set; }

        void Awake()
        {
            AutoBindRenderers();
            if (presets == null || presets.Length == 0)
                presets = BuildDefaultPresets();
            ApplyPreset(0);
        }

        void AutoBindRenderers()
        {
            if (bodyRenderer == null)
            {
                var robe = transform.Find("Robe");
                bodyRenderer = robe != null
                    ? robe.GetComponent<Renderer>()
                    : GetComponent<Renderer>();
            }
            if (hairRenderer == null)
            {
                var hair = transform.Find("Hair");
                if (hair != null) hairRenderer = hair.GetComponent<Renderer>();
            }
        }

        public AppearancePreset[] Presets => presets;

        public void ApplyPreset(int index)
        {
            if (presets == null || presets.Length == 0) return;
            PresetIndex = Mathf.Clamp(index, 0, presets.Length - 1);
            Current = presets[PresetIndex];
            ApplyColors(Current);
            PlayerSession.Appearance = Current;
        }

        public void Cycle(int delta)
        {
            if (presets == null || presets.Length == 0) return;
            int next = (PresetIndex + delta + presets.Length) % presets.Length;
            ApplyPreset(next);
        }

        public void ApplyColors(AppearancePreset preset)
        {
            if (preset == null) return;
            AutoBindRenderers();

            var skin = GetComponent<Renderer>();
            if (skin != null)
                SetColor(skin.material, preset.skinColor);

            if (bodyRenderer != null)
                SetColor(bodyRenderer.material, preset.outfitPrimary);

            if (hairRenderer != null)
            {
                SetColor(hairRenderer.material, preset.hairColor);
                var knot = hairRenderer.transform.Find("Topknot");
                if (knot != null)
                {
                    var kr = knot.GetComponent<Renderer>();
                    if (kr != null) SetColor(kr.material, Color.Lerp(preset.hairColor, Color.black, 0.25f));
                }
            }
        }

        public void ApplyArmorTint(Color tint)
        {
            AutoBindRenderers();
            if (bodyRenderer == null) return;
            var mat = bodyRenderer.material;
            var current = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
            SetColor(mat, Color.Lerp(current, tint, 0.65f));
        }

        static void SetColor(Material mat, Color color)
        {
            if (mat == null) return;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_ShadeColor"))
                mat.SetColor("_ShadeColor", Color.Lerp(color, new Color(0.12f, 0.1f, 0.14f), 0.5f));
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            mat.color = color;
        }

        static AppearancePreset[] BuildDefaultPresets()
        {
            return new[]
            {
                new AppearancePreset
                {
                    presetName = "Jade Scholar",
                    skinColor = new Color(0.93f, 0.8f, 0.7f),
                    hairColor = new Color(0.1f, 0.08f, 0.07f),
                    outfitPrimary = new Color(0.2f, 0.45f, 0.38f),
                    outfitSecondary = new Color(0.85f, 0.8f, 0.55f),
                    faceIndex = 0, hairIndex = 0
                },
                new AppearancePreset
                {
                    presetName = "Crimson Guard",
                    skinColor = new Color(0.88f, 0.72f, 0.58f),
                    hairColor = new Color(0.18f, 0.1f, 0.08f),
                    outfitPrimary = new Color(0.55f, 0.12f, 0.12f),
                    outfitSecondary = new Color(0.2f, 0.18f, 0.16f),
                    faceIndex = 1, hairIndex = 1
                },
                new AppearancePreset
                {
                    presetName = "Mist Wanderer",
                    skinColor = new Color(0.9f, 0.76f, 0.64f),
                    hairColor = new Color(0.35f, 0.32f, 0.3f),
                    outfitPrimary = new Color(0.25f, 0.3f, 0.4f),
                    outfitSecondary = new Color(0.55f, 0.58f, 0.62f),
                    faceIndex = 2, hairIndex = 2
                },
                new AppearancePreset
                {
                    presetName = "Bamboo Disciple",
                    skinColor = new Color(0.95f, 0.82f, 0.7f),
                    hairColor = new Color(0.08f, 0.07f, 0.06f),
                    outfitPrimary = new Color(0.35f, 0.42f, 0.22f),
                    outfitSecondary = new Color(0.7f, 0.62f, 0.4f),
                    faceIndex = 0, hairIndex = 3
                }
            };
        }
    }

    public static class PlayerSession
    {
        public static AppearancePreset Appearance;
        public static string StartingWeaponId = "weapon_jian";
        public static bool Created;
    }
}
