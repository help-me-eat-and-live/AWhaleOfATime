using UnityEngine;

public class OceanFloorGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain oceanTerrain;
    
    [Header("Ocean Floor Settings")]
    [Range(0.1f, 1.0f)]
    public float baseDepth = 0.3f; // Continental shelf depth (0-1 scale)
    
    [Header("Coral Reef Zones")]
    [Range(0.05f, 0.25f)]
    public float reefDepth = 0.15f; // Shallow coral areas
    [Range(10f, 50f)]
    public float reefRadius = 30f;
    
    [Header("Deep Ocean Trenches")]
    [Range(0.4f, 0.8f)]
    public float trenchDepth = 0.7f; // Deep trenches
    [Range(15f, 40f)]
    public float trenchRadius = 25f;
    
    [Header("Underwater Mountains")]
    [Range(0.1f, 0.4f)]
    public float mountainHeight = 0.2f;
    [Range(20f, 60f)]
    public float mountainRadius = 40f;

    void Start()
    {
        if (oceanTerrain == null)
        {
            oceanTerrain = GetComponent<Terrain>();
        }
        
        // Auto-generate ocean floor when script starts
        GenerateOceanFloor();
    }

    [ContextMenu("Generate Ocean Floor")]
    public void GenerateOceanFloor()
    {
        if (oceanTerrain == null)
        {
            Debug.LogError("No terrain assigned!");
            return;
        }

        TerrainData terrainData = oceanTerrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];

        // Step 1: Create base continental shelf
        CreateContinentalShelf(heights, width, height);

        // Step 2: Add coral reef zones
        CreateCoralReefs(heights, width, height);

        // Step 3: Add deep ocean trenches
        CreateDeepTrenches(heights, width, height);

        // Step 4: Add underwater mountains
        CreateUnderwaterMountains(heights, width, height);

        // Apply changes to terrain
        terrainData.SetHeights(0, 0, heights);
        
        Debug.Log("Ocean floor generated successfully!");
    }

    void CreateContinentalShelf(float[,] heights, int width, int height)
    {
        // Fill entire terrain with base continental shelf depth
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = baseDepth;
            }
        }
    }

    void CreateCoralReefs(float[,] heights, int width, int height)
    {
        // Reef Zone 1: Northeast Corner
        CreateReefZone(heights, width, height, 0.8f, 0.8f, reefRadius);
        
        // Reef Zone 2: Southwest Peninsula  
        CreateReefZone(heights, width, height, 0.3f, 0.4f, reefRadius * 0.8f);
        
        // Reef Zone 3: Western Barrier
        CreateReefZone(heights, width, height, 0.1f, 0.6f, reefRadius);
        
        // Reef Zone 4: Eastern Atoll
        CreateReefZone(heights, width, height, 0.75f, 0.3f, reefRadius * 0.7f);
        
        // Reef Zone 5: Southern Scattered Reefs
        CreateReefZone(heights, width, height, 0.6f, 0.2f, reefRadius * 0.5f);
        CreateReefZone(heights, width, height, 0.4f, 0.3f, reefRadius * 0.5f);
    }

    void CreateReefZone(float[,] heights, int width, int height, float centerX, float centerY, float radius)
    {
        int pixelCenterX = Mathf.RoundToInt(centerX * width);
        int pixelCenterY = Mathf.RoundToInt(centerY * height);
        int pixelRadius = Mathf.RoundToInt(radius * width / 200f); // Scale to terrain

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(pixelCenterX, pixelCenterY));
                
                if (distance < pixelRadius)
                {
                    // Create smooth falloff
                    float influence = 1f - (distance / pixelRadius);
                    influence = Mathf.SmoothStep(0f, 1f, influence);
                    
                    // Raise terrain to reef depth
                    heights[x, y] = Mathf.Lerp(heights[x, y], reefDepth, influence);
                }
            }
        }
    }

    void CreateDeepTrenches(float[,] heights, int width, int height)
    {
        // Deep Trench 1: Northwest Deep
        CreateTrench(heights, width, height, 0.3f, 0.7f, trenchRadius);
        
        // Deep Trench 2: Southeast Abyss
        CreateTrench(heights, width, height, 0.7f, 0.4f, trenchRadius);
    }

    void CreateTrench(float[,] heights, int width, int height, float centerX, float centerY, float radius)
    {
        int pixelCenterX = Mathf.RoundToInt(centerX * width);
        int pixelCenterY = Mathf.RoundToInt(centerY * height);
        int pixelRadius = Mathf.RoundToInt(radius * width / 200f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(pixelCenterX, pixelCenterY));
                
                if (distance < pixelRadius)
                {
                    float influence = 1f - (distance / pixelRadius);
                    influence = Mathf.SmoothStep(0f, 1f, influence);
                    
                    // Lower terrain to trench depth
                    heights[x, y] = Mathf.Lerp(heights[x, y], trenchDepth, influence);
                }
            }
        }
    }

    void CreateUnderwaterMountains(float[,] heights, int width, int height)
    {
        // Underwater Mountain 1: Central Peak
        CreateMountain(heights, width, height, 0.5f, 0.5f, mountainRadius);
        
        // Underwater Mountain 2: Eastern Ridge
        CreateMountain(heights, width, height, 0.65f, 0.7f, mountainRadius * 0.7f);
    }

    void CreateMountain(float[,] heights, int width, int height, float centerX, float centerY, float radius)
    {
        int pixelCenterX = Mathf.RoundToInt(centerX * width);
        int pixelCenterY = Mathf.RoundToInt(centerY * height);
        int pixelRadius = Mathf.RoundToInt(radius * width / 200f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(pixelCenterX, pixelCenterY));
                
                if (distance < pixelRadius)
                {
                    float influence = 1f - (distance / pixelRadius);
                    influence = Mathf.SmoothStep(0f, 1f, influence);
                    
                    // Raise terrain to mountain height
                    heights[x, y] = Mathf.Lerp(heights[x, y], mountainHeight, influence);
                }
            }
        }
    }
}
