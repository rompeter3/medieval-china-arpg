using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MedievalChinaARPG.Editor
{
    /// <summary>
    /// Headless / menu player builds for Windows and macOS.
    /// GameCI invokes BuildAll via -executeMethod when customParameters are set;
    /// unity-builder's default path also works via EditorBuildSettings scenes.
    /// </summary>
    public static class BuildPlayers
    {
        const string WindowsDir = "Builds/Windows";
        const string MacDir = "Builds/macOS";
        const string WindowsExe = "MedievalChinaARPG.exe";
        const string MacApp = "MedievalChinaARPG.app";

        static readonly string[] ScenePaths =
        {
            "Assets/Scenes/Bootstrap.unity",
            "Assets/Scenes/CharacterCreate.unity",
            "Assets/Scenes/HubTown.unity",
            "Assets/Scenes/CombatLevel.unity",
        };

        [MenuItem("Build/Players/Windows + macOS")]
        public static void BuildAll()
        {
            BuildWindows();
            BuildMac();
            Debug.Log("BuildPlayers: Windows + macOS complete.");
        }

        [MenuItem("Build/Players/Windows")]
        public static void BuildWindows()
        {
            BuildOne(BuildTarget.StandaloneWindows64, WindowsDir, WindowsExe);
        }

        [MenuItem("Build/Players/macOS")]
        public static void BuildMac()
        {
            BuildOne(BuildTarget.StandaloneOSX, MacDir, MacApp);
        }

        static void BuildOne(BuildTarget target, string outDir, string fileName)
        {
            // Mono enables Linux CI cross-compiles (GameCI windows-mono / mac-mono images).
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);

            Directory.CreateDirectory(outDir);
            var location = Path.Combine(outDir, fileName);
            var scenes = ScenePaths.Where(File.Exists).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("BuildPlayers: no scenes found under Assets/Scenes.");

            var opts = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = location,
                target = target,
                options = BuildOptions.CompressWithLz4HC,
            };

            var report = BuildPipeline.BuildPlayer(opts);
            var summary = report.summary;
            if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new InvalidOperationException(
                    $"BuildPlayers: {target} failed ({summary.totalErrors} errors).");

            Debug.Log($"BuildPlayers: {target} → {location} ({summary.totalSize} bytes)");
        }
    }
}
