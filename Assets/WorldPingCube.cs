using UnityEngine;

public class WorldPingCube : MonoBehaviour
{
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 2, 5);
        cube.transform.localScale = Vector3.one * 5f;

        var renderer = cube.GetComponent<Renderer>();
        if (renderer)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.cyan;
            renderer.material = mat;
        }

        Debug.Log("🧊 Cube spawned at (0,2,5)");
    }
}
