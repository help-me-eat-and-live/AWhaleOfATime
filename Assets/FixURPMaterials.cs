using UnityEngine;

public class FixURPMaterials : MonoBehaviour
{
    public Material fallbackMaterial;

    void Start()
    {
        if (fallbackMaterial == null)
        {
            Debug.LogError("❌ Please assign a fallback Material in the inspector.");
            return;
        }

        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        int converted = 0;

        foreach (Renderer r in renderers)
        {
            if (r.sharedMaterial != null && r.sharedMaterial.shader.name.Contains("Universal"))
            {
                r.sharedMaterial = fallbackMaterial;
                converted++;
                Debug.Log($"🔁 Replaced material on {r.name}");
            }
        }

        Debug.Log($"✅ FixURPMaterials: Converted {converted} URP materials.");
    }
}
