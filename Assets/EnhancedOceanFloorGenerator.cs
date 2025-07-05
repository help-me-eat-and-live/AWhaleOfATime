using UnityEngine;

[System.Serializable]
public class BiomeTexture
{
    public string biomeName;
    public int terrainLayerIndex;
    public Vector2 worldCenter;
    public float radius;
    public AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public float baseStrength = 1f;
}

public class EnhancedOceanFloorGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain oceanTerrain;
    
    [Header("Integration with Existing Assets")]
    public bool useSUIMONOWater = true;
    public bool useURPWaterShader = false;
    
    [Header("Terrain Texture Layers")]
    public TerrainLayer[] terrainTextureLayers = new TerrainLayer[4];
    
    [Header("Ocean Floor Settings")]
    public float baseDepth = 0.3f;
    
    [Header("Biome Textures")]
    public BiomeTexture[] biomeTextures = new BiomeTexture[4];
    
    void Start()
    {
        // Auto-generate on start - you can comment this out if you don't want it
        // GenerateOceanFloor();
    }
    
    [ContextMenu("Refresh Ocean Floor")]
    public void GenerateOceanFloor()
    {
        if (oceanTerrain == null)
        {
            Debug.LogError("Ocean Terrain not assigned!");
            return;
        }
        
        // Fix falloff curves if they're broken
        for (int i = 0; i < biomeTextures.Length; i++)
        {
            if (biomeTextures[i] != null)
            {
                // Check if curve is broken (should return 1.0 at input 1.0, and 0.0 at input 0.0)
                float valueAtOne = biomeTextures[i].falloffCurve.Evaluate(1f);
                float valueAtZero = biomeTextures[i].falloffCurve.Evaluate(0f);
                
                if (valueAtOne < 0.9f || valueAtZero > 0.1f)
                {
                    Debug.Log($"Fixing broken falloff curve for {biomeTextures[i].biomeName} (was {valueAtOne:F2} at 1.0, {valueAtZero:F2} at 0.0)");
                    
                    // Create proper falloff curve: at input 1.0 -> output 1.0, at input 0.0 -> output 0.0
                    AnimationCurve newCurve = new AnimationCurve();
                    newCurve.AddKey(0f, 0f);  // At input 0, output 0
                    newCurve.AddKey(1f, 1f);  // At input 1, output 1
                    
                    biomeTextures[i].falloffCurve = newCurve;
                }
            }
        }
        
        TerrainData terrainData = oceanTerrain.terrainData;
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        
        // Get the current alphamap
        float[,,] alphamap = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        
        // Clear existing alphamap
        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                {
                    alphamap[y, x, layer] = 0f;
                }
            }
        }
        
        // Generate new texture distribution
        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                // Convert alphamap coordinates to world position
                float worldX = (float)x / alphamapWidth * terrainData.size.x + oceanTerrain.transform.position.x;
                float worldZ = (float)y / alphamapHeight * terrainData.size.z + oceanTerrain.transform.position.z;
                Vector2 worldPos = new Vector2(worldX, worldZ);
                
                // Calculate weights for each biome
                float[] weights = new float[biomeTextures.Length];
                float totalWeight = 0f;
                
                // Debug center point (0,0 in world coords should be center of terrain)
                bool isCenter = (x == alphamapWidth/2 && y == alphamapHeight/2);
                
                for (int i = 0; i < biomeTextures.Length; i++)
                {
                    if (biomeTextures[i] != null)
                    {
                        float distance = Vector2.Distance(worldPos, biomeTextures[i].worldCenter);
                        float normalizedDistance = Mathf.Clamp01(distance / biomeTextures[i].radius);
                        
                        // Use falloff curve to determine influence
                        float influence = biomeTextures[i].falloffCurve.Evaluate(1f - normalizedDistance);
                        weights[i] = influence * biomeTextures[i].baseStrength;
                        totalWeight += weights[i];
                        
                        // Debug output for center point
                        if (isCenter)
                        {
                            Debug.Log($"Center point - Biome {i} ({biomeTextures[i].biomeName}): distance={distance:F1}, normalizedDist={normalizedDistance:F2}, influence={influence:F3}, weight={weights[i]:F3}");
                        }
                    }
                }
                
                // If no biome has significant influence, default to sand layer (Layer 0)
                if (totalWeight < 0.01f)
                {
                    // Clear all weights first
                    for (int j = 0; j < weights.Length; j++)
                    {
                        weights[j] = 0f;
                    }
                    // Find which biome uses Layer 0 (sand) and set that weight
                    for (int j = 0; j < biomeTextures.Length; j++)
                    {
                        if (biomeTextures[j] != null && biomeTextures[j].terrainLayerIndex == 0)
                        {
                            weights[j] = 1f;
                            totalWeight = 1f;
                            break;
                        }
                    }
                    // If no biome uses Layer 0, just use the first layer
                    if (totalWeight < 0.01f)
                    {
                        weights[0] = 1f;
                        totalWeight = 1f;
                    }
                }
                
                // Normalize weights and apply to alphamap
                for (int i = 0; i < biomeTextures.Length; i++)
                {
                    if (biomeTextures[i] != null && biomeTextures[i].terrainLayerIndex < terrainData.alphamapLayers)
                    {
                        float normalizedWeight = weights[i] / totalWeight;
                        alphamap[y, x, biomeTextures[i].terrainLayerIndex] = normalizedWeight;
                    }
                }
            }
        }
        
        // Apply the new alphamap
        terrainData.SetAlphamaps(0, 0, alphamap);
        
        // Debug information
        Debug.Log("Ocean floor generation complete! Biome textures applied based on world positions.");
        Debug.Log($"Terrain has {terrainData.alphamapLayers} texture layers");
        Debug.Log($"Terrain world position: {oceanTerrain.transform.position}");
        Debug.Log($"Terrain size: {terrainData.size}");
        
        for (int i = 0; i < biomeTextures.Length; i++)
        {
            if (biomeTextures[i] != null)
            {
                Debug.Log($"Biome {i}: {biomeTextures[i].biomeName} -> Layer {biomeTextures[i].terrainLayerIndex}, Center: {biomeTextures[i].worldCenter}, Radius: {biomeTextures[i].radius}");
            }
        }
    }
    
    // Method to visualize biome boundaries in Scene view
    void OnDrawGizmosSelected()
    {
        if (biomeTextures == null) return;
        
        for (int i = 0; i < biomeTextures.Length; i++)
        {
            if (biomeTextures[i] != null)
            {
                // Set different colors for each biome
                Color[] colors = { Color.yellow, Color.green, Color.brown, Color.gray };
                Gizmos.color = colors[i % colors.Length];
                
                // Draw biome center and radius
                Vector3 center = new Vector3(biomeTextures[i].worldCenter.x, 0, biomeTextures[i].worldCenter.y);
                Gizmos.DrawWireSphere(center, biomeTextures[i].radius);
                
                // Draw center point
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(center, 5f);
            }
        }
    }
}
