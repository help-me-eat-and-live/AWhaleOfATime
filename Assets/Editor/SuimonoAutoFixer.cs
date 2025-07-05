// Assets/Editor/SuimonoAutoFixer.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class SuimonoAutoFixer : EditorWindow
{
    [MenuItem("Tools/Suimono/Auto-Fix Deprecated Usages")]
    public static void FixDeprecatedUsages()
    {
        string suimonoPath = "Assets/SUIMONO - WATER SYSTEM 2/SCRIPTS";
        string[] files = Directory.GetFiles(suimonoPath, "*.cs", SearchOption.AllDirectories);

        int changes = 0;
        foreach (string filePath in files)
        {
            string code = File.ReadAllText(filePath);
            string original = code;

            // Fix: FindObjectOfType(typeof(Type)) → (Type)FindFirstObjectByType<Type>()
            code = Regex.Replace(code,
                @"FindObjectOfType\(typeof\(([^\)]+)\)\)",
                m => $"({m.Groups[1].Value})FindFirstObjectByType<{m.Groups[1].Value}>()"
            );

            // Fix: FindObjectOfType<Type>() → FindFirstObjectByType<Type>()
            code = Regex.Replace(code,
                @"FindObjectOfType<([^>]+)>\(\)",
                m => $"FindFirstObjectByType<{m.Groups[1].Value}>()"
            );

            // Fix: FindObjectsOfType<T>() → FindObjectsByType<T>(FindObjectsSortMode.None)
            code = Regex.Replace(code,
                @"FindObjectsOfType<([^>]+)>\(\)",
                m => $"FindObjectsByType<{m.Groups[1].Value}>(FindObjectsSortMode.None)"
            );

            // Fix: ParticleSystem.startColor → main.startColor
            code = Regex.Replace(code,
                @"(\w+)\.startColor\s*=\s*([^;]+);",
                m => $"var main = {m.Groups[1].Value}.main;\nmain.startColor = {m.Groups[2].Value};"
            );

            // Fix: RenderingPath.DeferredShading → RenderingPath.DeferredShading
            code = Regex.Replace(code,
                @"RenderingPath\.DeferredLighting",
                "RenderingPath.DeferredShading"
            );

            if (code != original)
            {
                File.WriteAllText(filePath, code);
                Debug.Log($"✅ Patched: {Path.GetFileName(filePath)}");
                changes++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ Auto-fix complete. {changes} file(s) updated.");
    }

    [MenuItem("Tools/Suimono/Validate Prefabs in Scene")]
    public static void ValidateScene()
    {
        var modules = GameObject.FindObjectsByType<Suimono.Core.SuimonoModule>(FindObjectsSortMode.None);
        if (modules.Length == 0)
        {
            Debug.LogError("❌ No SuimonoModule found in scene!");
            return;
        }

        foreach (var module in modules)
        {
            var lib = module.GetComponent<Suimono.Core.SuimonoModuleLib>();
            if (lib == null)
                Debug.LogError($"❌ SuimonoModuleLib missing on: {module.gameObject.name}");
            else if (lib.materialSurface == null)
                Debug.LogError($"❌ 'materialSurface' not assigned in SuimonoModuleLib on: {module.gameObject.name}");
            else
                Debug.Log($"✅ {module.gameObject.name} is properly configured.");
        }
    }
}
