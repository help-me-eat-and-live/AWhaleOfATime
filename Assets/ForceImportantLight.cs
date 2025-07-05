using UnityEngine;

[ExecuteAlways]
public class ForceImportantLight : MonoBehaviour
{
    void OnEnable()
    {
        var light = GetComponent<Light>();
        if (light != null)
            light.renderMode = LightRenderMode.ForcePixel; // Equivalent to "Important"
    }
}
