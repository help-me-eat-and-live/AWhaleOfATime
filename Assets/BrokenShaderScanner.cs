using UnityEngine;

public class BrokenShaderScanner : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 BrokenShaderScanner is running...");

        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        Debug.Log($"🔢 Found {renderers.Length} renderers.");

        foreach (Renderer r in renderers)
        {
            if (r.sharedMaterial && r.sharedMaterial.shader.name.Contains("Universal"))
            {
                Debug.LogWarning($"🚫 {r.name} uses: {r.sharedMaterial.shader.name}");
            }
            else if (r.sharedMaterial == null)
            {
                Debug.LogWarning($"⚠️ {r.name} has NO material!");
            }
            else
            {
                Debug.Log($"✅ {r.name} uses: {r.sharedMaterial.shader.name}");
            }
        }
    }
}
