using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MedievalChinaARPG.Editor
{
    /// <summary>
    /// Dumps Unity Console entries to Temp/CONSOLE_DUMP.txt when Temp/DUMP_CONSOLE exists.
    /// </summary>
    [InitializeOnLoad]
    public static class DumpConsoleEntries
    {
        const string TriggerName = "DUMP_CONSOLE";
        const string OutName = "CONSOLE_DUMP.txt";
        static readonly List<string> s_live = new List<string>();

        static DumpConsoleEntries()
        {
            Application.logMessageReceivedThreaded += OnLog;
            EditorApplication.update += Poll;
        }

        static string TriggerPath => Path.Combine(Application.dataPath, "..", "Temp", TriggerName);
        static string OutPath => Path.Combine(Application.dataPath, "..", "Temp", OutName);

        static void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log) return;
            lock (s_live)
            {
                s_live.Add(type + " | " + condition.Replace('\n', ' '));
                if (s_live.Count > 200) s_live.RemoveAt(0);
            }
        }

        static void Poll()
        {
            if (!File.Exists(TriggerPath))
                return;

            try { File.Delete(TriggerPath); } catch { /* ignore */ }

            try
            {
                var text = Dump();
                File.WriteAllText(OutPath, text);
                Debug.Log("DumpConsoleEntries wrote " + OutPath);
            }
            catch (Exception ex)
            {
                File.WriteAllText(OutPath, "DUMP_FAILED: " + ex);
            }
        }

        static string Dump()
        {
            var sb = new StringBuilder();
            DumpLogEntries(sb);
            DumpLive(sb);
            DumpShaders(sb);
            DumpUrp(sb);
            return sb.ToString();
        }

        static void DumpLive(StringBuilder sb)
        {
            sb.AppendLine("==== LIVE_LOGS (errors/asserts/exceptions) ====");
            lock (s_live)
            {
                sb.AppendLine("LIVE_COUNT=" + s_live.Count);
                foreach (var line in s_live)
                    sb.AppendLine(line);
            }
        }

        static void DumpShaders(StringBuilder sb)
        {
            sb.AppendLine("==== SHADERS ====");
            string[] names =
            {
                "MedievalChinaARPG/StylizedRim",
                "MedievalChinaARPG/StylizedFoliage",
                "MedievalChinaARPG/StylizedWater",
                "MedievalChinaARPG/StylizedEmissive",
                "Universal Render Pipeline/Lit",
                "Universal Render Pipeline/Unlit"
            };
            foreach (var n in names)
            {
                var s = Shader.Find(n);
                sb.AppendLine(n + " => " + (s == null ? "NULL" : ("found supported=" + s.isSupported)));
                if (s == null) continue;
                try
                {
                    var messages = ShaderUtil.GetShaderMessages(s);
                    if (messages != null)
                    {
                        foreach (var m in messages)
                            sb.AppendLine("  MSG " + m.severity + " " + m.message);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine("  MSG_FAIL " + ex.Message);
                }
            }
        }

        static void DumpUrp(StringBuilder sb)
        {
            sb.AppendLine("==== URP ====");
            sb.AppendLine("currentPipeline=" + (GraphicsSettings.currentRenderPipeline != null ? GraphicsSettings.currentRenderPipeline.name : "NULL"));
            sb.AppendLine("defaultPipeline=" + (GraphicsSettings.defaultRenderPipeline != null ? GraphicsSettings.defaultRenderPipeline.name : "NULL"));
            var asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (asset != null)
                sb.AppendLine("urpAsset=" + asset.name);
        }

        static void DumpLogEntries(StringBuilder sb)
        {
            var logEntriesType = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll")
                ?? Type.GetType("UnityEditor.LogEntries,UnityEditor");
            if (logEntriesType == null)
            {
                sb.AppendLine("LogEntries type not found");
                return;
            }

            var getCountsByType = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
            if (getCountsByType != null)
            {
                object[] cargs = { 0, 0, 0 };
                getCountsByType.Invoke(null, cargs);
                sb.AppendLine("COUNTS_BY_TYPE error=" + cargs[0] + " warning=" + cargs[1] + " log=" + cargs[2]);
            }

            var getCount = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
            var getEntry = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
            var startGetting = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public);
            var endGetting = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
            var entryType = Type.GetType("UnityEditor.LogEntry,UnityEditor.dll")
                ?? Type.GetType("UnityEditor.LogEntry,UnityEditor");
            if (entryType == null)
            {
                sb.AppendLine("LogEntry type not found");
                return;
            }

            var entry = Activator.CreateInstance(entryType);
            var messageField = entryType.GetField("message");
            var modeField = entryType.GetField("mode");

            startGetting?.Invoke(null, null);
            int count = (int)(getCount?.Invoke(null, null) ?? 0);
            sb.AppendLine("COUNT=" + count);
            sb.AppendLine("==== LOG_ENTRIES ====");

            for (int i = 0; i < count; i++)
            {
                var args = new object[] { i, entry };
                bool ok = (bool)(getEntry?.Invoke(null, args) ?? false);
                entry = args[1];
                if (!ok) continue;
                string message = messageField?.GetValue(entry) as string ?? "";
                int mode = modeField != null ? Convert.ToInt32(modeField.GetValue(entry)) : 0;
                sb.AppendLine("MODE=" + mode);
                sb.AppendLine(message);
                sb.AppendLine("----");
            }

            endGetting?.Invoke(null, null);
        }
    }
}
