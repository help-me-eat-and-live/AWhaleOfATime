using UnityEngine;

public class BiomeDebugLogger : MonoBehaviour
{
    public EnhancedOceanFloorGenerator generator; // <- draggable now

    [ContextMenu("Log Biome & Terrain Info")]
    public void LogBiomeAndTerrainInfo()
    {
        if (generator == null || generator.oceanTerrain == null)
        {
            Debug.LogError("Missing reference to generator or terrain.");
            return;
        }

        var terrain = generator.oceanTerrain;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        Vector2 min = new Vector2(terrainPos.x, terrainPos.z);
        Vector2 max = min + new Vector2(terrainSize.x, terrainSize.z);

        Debug.Log($"🧭 Terrain bounds: X({min.x} to {max.x}), Z({min.y} to {max.y})");

        foreach (var biome in generator.biomeTextures)
        {
            var pos = biome.worldCenter;
            string result = (pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y) 
                ? "✅ Inside bounds"
                : "❌ Outside bounds";

            Debug.Log($"Biome '{biome.biomeName}' at {pos} → {result}");
        }
    }
}
